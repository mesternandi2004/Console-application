🏠 Household Register – Food Storage Management System
Overview

This project was developed as part of the Advanced Software Development course for the 2024/25 semester. The aim is to create a layered application that manages food storage in a household, including both pantry and refrigerator items. The system handles food inventory, storage capacities, household members, and notifications related to stock levels and preferences.
📝 Assignment Summary

The Household Register system allows for:

    Tracking and managing food items stored in the pantry and refrigerator.

    Managing household members and their food preferences.

    Monitoring storage capacities to prevent overfilling.

    Automatically notifying users in certain situations (e.g., low stock or favorite item restocked).

    Performing queries on stored data and exporting current stock to a .txt file.

✅ Task Requirements
📥 Data Loading

    Load data from a structured JSON file into a relational database.

    Support manual data entry if no JSON is provided.

    JSON format includes food items and household members.

🛠️ Data Handling

    Add new items manually.

    Ensure capacity limits are respected in pantry and refrigerator.

    Update quantities when items are used.

    Trigger events when:

        A food item drops below a critical threshold.

        A favorite food of a person is restocked.

👨‍👩‍👧 Household Member Management

    Add, list, and remove household members.

    Assign fixed food preferences to each person (unchangeable).

📊 Statistics and Queries

Generate reports such as:

    Items close to expiration.

    Items low in stock.

    Remaining storage capacity, with user notification when capacity becomes critical.

    List in-stock items (non-zero quantity), exportable to .txt.
