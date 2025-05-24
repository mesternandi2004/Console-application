using Spectre.Console.Cli;
using Spectre.Console;
using System.ComponentModel;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;
using CQW1QQ_HSZF_2024251.Models;
using CQW1QQ_HSZF_2024251.Application.Interfaces;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace CQW1QQ_HSZF_2024251.Console.Commands
{
    public class StartCommand(IProductService productService, IStorageService storageService, IPantryRepository pantryRepository, IFridgeRepository fridgeRepository, IProductRepository productRepository, IPersonRepository personRepository) : Command<StartCommand.Settings>
    {
        private readonly IProductService _productService = productService;
        private readonly IStorageService _storageService = storageService;

        private readonly IPantryRepository _pantryRepository = pantryRepository;
        private readonly IFridgeRepository _fridgeRepository = fridgeRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IPersonRepository _personRepository = personRepository;

        public class Settings : CommandSettings
        {
            [CommandOption("-p|--path <path>")]
            [Description("The path to the JSON file containing the starting database.")]
            public string Path { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(settings.Path))
                {
                    if (!File.Exists(settings.Path))
                    {
                        AnsiConsole.MarkupLine("[red]Error: The specified JSON file does not exist.[/]");
                        return -1;
                    }

                    string jsonData = File.ReadAllText(settings.Path);
                    var database = JsonSerializer.Deserialize<Household>(jsonData);

                    if (database != null)
                    {
                        AnsiConsole.MarkupLine("[green]Database loaded successfully from JSON file.[/]");
                        LoadDatabase(database);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Error: Failed to parse the JSON file.[/]");
                        return -1;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]No path specified. Initializing with an empty database.[/]");
                    InitializeEmptyDatabase();
                }

                StartMenu();
                return 0;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                return -1;
            }
        }

        private void LoadDatabase(Household database)
        {
            _fridgeRepository.Create(database.Fridge);
            _pantryRepository.Create(database.Pantry);

            foreach (var product in database.Products)
            {
                Product acualProduct = product;
                _storageService.PutAwayProduct(ref acualProduct);
                _productRepository.Create(acualProduct);
            }
            foreach (var person in database.Persons)
            {
                _personRepository.Create(person);
            }
        }

        private void InitializeEmptyDatabase()
        {
            _fridgeRepository.Create(new Fridge(100));
            _pantryRepository.Create(new Pantry(100));
        }

        private void StartMenu()
        {
            AnsiConsole.Clear();
            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]
                    {
                        "Manage Foods (CRUD)",
                        "Manage Household Members (CRUD)",
                        "Export Inventory to File",
                        "List Near Expiry Foods",
                        "Check Remaining Capacity",
                        "Exit"
                    })
            );

            switch (selectedOption)
            {
                case "Manage Foods (CRUD)":
                    ManageFoods();
                    break;

                case "Manage Household Members (CRUD)":
                    ManageHouseholdMembers();
                    break;

                case "Export Inventory to File":
                    ExportInventoryToFile();
                    break;

                case "List Near Expiry Foods":
                    ListNearExpiryFoods();
                    break;

                case "Check Remaining Capacity":
                    CheckRemainingCapacity();
                    break;

                case "Exit":
                    AnsiConsole.MarkupLine("[red]Exiting... Goodbye![/]");
                    break;
            }
        }

        private void ManageFoods()
        {
            AnsiConsole.Clear();

            var selectedOption = AnsiConsole.Prompt(
               new SelectionPrompt<string>()
                   .AddChoices(new[]
                   {
                        "List all products",
                        "Put away new product",
                        "Use product",
                        "Restock existing product",
                        "Back to Main Menu"
                   })
           );

            switch (selectedOption)
            {
                case "List all products":
                    ListProducts();
                    break;

                case "Put away new product":
                    PutAwayProduct();
                    break;

                case "Use product":
                    UseProduct();
                    break;

                case "Restock existing product":
                    RestockProduct();
                    break;

                case "Back to Main Menu":
                    StartMenu();
                    break;
            }
        }

        private void ListProducts()
        {
            AnsiConsole.Clear();

            var grid = new Grid();

            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();

            grid.AddRow(new string[] { "Id", "Name", "Quantity", "Critical level", "Best before", "Stored" });

            var products = _productRepository.ReadAll();
            foreach (var product in products)
            {
                var productRow = new string[]
                {
                    product.Id.ToString(),
                    product.Name.ToString(),
                    product.Quantity.ToString(),
                    product.CriticalLevel.ToString(),
                    product.BestBefore.ToString(),
                    $"{(product.StoreInFridge ? "Fridge" : "Pantry")}"
                };

                grid.AddRow(productRow);
            }
            AnsiConsole.Write(grid);
            Back();
        }
        private void Back()
        {
            var selectedOption = AnsiConsole.Prompt(
               new SelectionPrompt<string>()
                   .AddChoices(new[]
                   {
                        "Back to Main Menu",
                        "Exit"
                   })
           );

            switch (selectedOption)
            {
                case "Back to Main Menu":
                    StartMenu();
                    break;

                case "Exit":
                    AnsiConsole.MarkupLine("[red]Exiting... Goodbye![/]");
                    break;
            }
        }

        private void RestockProduct()
        {
            AnsiConsole.Clear();

            var products = _productRepository.ReadAll();
            if (!products.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No products found![/]");
                Back();
                return;
            }

            var selectedProduct = AnsiConsole.Prompt(
                new SelectionPrompt<Product>()
                    .Title("Select a product to restock:")
                    .AddChoices(products)
                    .UseConverter(product => $"{product.Name} (ID: {product.Id}) - Quantity: {product.Quantity}"));

            var quantity = AnsiConsole.Prompt(new TextPrompt<int>($"How much of {selectedProduct.Name} do you want to add?")
                .ValidationErrorMessage("[red]Invalid quantity. Please enter a positive number.[/]")
                .Validate(qty => qty > 0));

            try
            {
                _productService.RestockProduct(selectedProduct.Id, quantity);
                AnsiConsole.MarkupLine($"[green]Successfully restocked {quantity} of {selectedProduct.Name}.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error restocking product: {ex.Message}[/]");
            }
            Back();

        }

        private void UseProduct()
        {
            AnsiConsole.Clear();

            var products = _productRepository.ReadAll();
            if (!products.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No products found![/]");
                Back();
                return;
            }

            var selectedProduct = AnsiConsole.Prompt(
                new SelectionPrompt<Product>()
                    .Title("Select a product to use:")
                    .AddChoices(products)
                    .UseConverter(product => $"{product.Name} (ID: {product.Id}) - Quantity: {product.Quantity}"));

            var quantity = AnsiConsole.Prompt(new TextPrompt<int>($"How much of {selectedProduct.Name} do you want to use?")
                .ValidationErrorMessage("[red]Invalid quantity. Please enter a positive number.[/]")
                .Validate(qty => qty > 0));

            try
            {
                _productService.ConsumeProduct(selectedProduct.Id, quantity);
                AnsiConsole.MarkupLine($"[green]Successfully used {quantity} of {selectedProduct.Name}.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error using product: {ex.Message}[/]");
            }

            Back();

        }

        private void PutAwayProduct()
        {
            AnsiConsole.Clear();

            var name = AnsiConsole.Prompt(new TextPrompt<string>("Name:"));
            var quantity = AnsiConsole.Prompt(new TextPrompt<double>("Quantity:"));
            var criticalLevel = AnsiConsole.Prompt(new TextPrompt<double>("Critical level:"));
            var bestBefore = AnsiConsole.Prompt(new TextPrompt<DateTime>("Best before:"));
            var storeInFridge = AnsiConsole.Prompt(
                new TextPrompt<bool>("Put it in fridge?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "y" : "n"));
            var nextId = _productRepository.ReadAll().Max(x => x.Id) + 1;

            Product product = new(nextId, name, quantity, criticalLevel, bestBefore, storeInFridge);

            _storageService.PutAwayProduct(ref product);
            _productRepository.Create(product);
            Back();
        }
        private void ManageHouseholdMembers()
        {
            AnsiConsole.Clear();

            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[]
                    {
                "List Household Members",
                "Add New Household Member",
                "Remove Household Member",
                "Update Household Member",
                "Back to Main Menu"
                    })
            );

            switch (selectedOption)
            {
                case "List Household Members":
                    ListHouseholdMembers();
                    break;

                case "Add New Household Member":
                    AddNewHouseholdMember();
                    break;

                case "Remove Household Member":
                    RemoveHouseholdMember();
                    break;

                case "Update Household Member":
                    UpdateHouseholdMember();
                    break;

                case "Back to Main Menu":
                    StartMenu();
                    break;
            }
        }

        private void ListHouseholdMembers()
        {
            AnsiConsole.Clear();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();

            grid.AddRow(new[] { "Name", "ResponsibleForPurchase", "Favorite Product IDs" });

            var persons = _personRepository.ReadAll();
            foreach (var person in persons)
            {
                grid.AddRow(new[]
                {
                    person.Name,
                    person.ResponsibleForPurchase ? "Yes" : "No",
                    string.Join(", ", person.FavoriteProductIds)
                });
            }

            AnsiConsole.Write(grid);
            Back();
        }

        private void AddNewHouseholdMember()
        {
            AnsiConsole.Clear();

            var name = AnsiConsole.Prompt(new TextPrompt<string>("Enter the name of the new household member:"));
            var responsibleForPurchase = AnsiConsole.Prompt(
                new TextPrompt<bool>("Is this member responsible for the purchase?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "y" : "n"));

            var favoriteProducts = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the favorite product IDs (comma-separated):")
                    .AllowEmpty());

            var favoriteProductIds = favoriteProducts
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            Person newPerson = new(name, favoriteProductIds, responsibleForPurchase, new List<Product>());
            _personRepository.Create(newPerson);

            AnsiConsole.MarkupLine("[green]Household member added successfully![/]");
            Back();
        }

        private void RemoveHouseholdMember()
        {
            AnsiConsole.Clear();

            var persons = _personRepository.ReadAll();
            if (!persons.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No household members found![/]");
                Back();
                return;
            }

            var personToRemove = AnsiConsole.Prompt(
                new SelectionPrompt<Person>()
                    .Title("Select a household member to remove:")
                    .AddChoices(persons));

            _personRepository.Delete(personToRemove);
            AnsiConsole.MarkupLine($"[green]Household member {personToRemove.Name} removed successfully![/]");
            Back();
        }

        private void UpdateHouseholdMember()
        {
            AnsiConsole.Clear();

            var persons = _personRepository.ReadAll();
            if (!persons.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No household members found![/]");
                Back();
                return;
            }

            var personToUpdate = AnsiConsole.Prompt(
                new SelectionPrompt<Person>()
                    .Title("Select a household member to update:")
                    .AddChoices(persons));

            var newName = AnsiConsole.Prompt(new TextPrompt<string>($"Enter a new name for {personToUpdate.Name} :"));
            if (!string.IsNullOrWhiteSpace(newName))
            {
                personToUpdate.Name = newName;
            }

            var responsibleForPurchase = AnsiConsole.Prompt(
                new TextPrompt<bool>("Is this member responsible for the purchase? (y/n):")
                    .DefaultValue(personToUpdate.ResponsibleForPurchase));

            personToUpdate.ResponsibleForPurchase = responsibleForPurchase;

            var favoriteProducts = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the favorite product IDs (comma-separated):")
                    .AllowEmpty());

            if (!string.IsNullOrWhiteSpace(favoriteProducts))
            {
                personToUpdate.FavoriteProductIds = favoriteProducts
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }

            _personRepository.Update(personToUpdate);
            AnsiConsole.MarkupLine($"[green]Household member {personToUpdate.Name} updated successfully![/]");
            Back();
        }

        private void ExportInventoryToFile()
        {
            AnsiConsole.Clear();

            var date = DateTime.Now.ToString("ddMMyyyy");
            var timestamp = DateTime.Now.ToString("HHmmss");

            var exportDirectory = Path.Combine("Exports", date);

            try
            {
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }

                var filePath = Path.Combine(exportDirectory, $"HouseholdRegisterExport_{timestamp}.txt");

                using (var writer = new StreamWriter(filePath))
                {
                    var products = _productRepository.ReadAll();
                    writer.WriteLine("Id\tName\tQuantity\tCritical Level\tBest Before\tStored In");

                    foreach (var product in products)
                    {
                        writer.WriteLine($"{product.Id}\t{product.Name}\t{product.Quantity}\t{product.CriticalLevel}\t{product.BestBefore}\t{(product.StoreInFridge ? "Fridge" : "Pantry")}");
                    }
                }

                AnsiConsole.MarkupLine($"[green]Inventory successfully exported to {filePath}![/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error exporting inventory: {ex.Message}[/]");
            }

            Back();
        }



        private void ListNearExpiryFoods()
        {
            var daysToExpire = AnsiConsole.Prompt(new TextPrompt<int>("In what time peroid to check?(days)"));
            var expiry = _productService.GetNearExpiryProducts(daysToExpire);

            AnsiConsole.Clear();

            var grid = new Grid();

            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();
            grid.AddColumn();

            grid.AddRow(new string[] { "Id", "Name", "Quantity", "Critical level", "Best before", "Stored" });
            foreach (var product in expiry)
            {
                var productRow = new string[]
                {
                    product.Id.ToString(),
                    product.Name.ToString(),
                    product.Quantity.ToString(),
                    product.CriticalLevel.ToString(),
                    product.BestBefore.ToString(),
                    $"{(product.StoreInFridge ? "Fridge" : "Pantry")}"
                };

                grid.AddRow(productRow);
            }
            AnsiConsole.Write(grid);
            Back();
        }

        private void CheckRemainingCapacity()
        {
            AnsiConsole.Clear();

            var fridge = _fridgeRepository.ReadAll().FirstOrDefault();
            var pantry = _pantryRepository.ReadAll().FirstOrDefault();

            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            grid.AddRow(new string[] { "Name", "Remaining capacity" });

            var capacitryFridge = _storageService.GetRemainingCapacity(fridge);
            var capacitryPantry = _storageService.GetRemainingCapacity(pantry);


            grid.AddRow(new string[] { "Fridge", capacitryFridge.ToString() });
            grid.AddRow(new string[] { "Pantry", capacitryPantry.ToString() });
            AnsiConsole.Write(grid);

            Back();
        }
    }
}
