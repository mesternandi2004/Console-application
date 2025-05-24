using CQW1QQ_HSZF_2024251.Models;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Interfaces;
using CQW1QQ_HSZF_2024251.Persistence.MsSql.Repository;
using Microsoft.EntityFrameworkCore;

namespace CQW1QQ_HSZF_2024251.Persistence.MsSql
{
    public class PantryRepository(DbContext context) : Repository<Pantry>(context), IPantryRepository
    {
    }
}
