using System;
using System.Collections;
namespace VillaManager.Services.Helper;

public static class Helper
{
    // Generating Unique String Key
    public static string GenerateUniquePermissionTag()
    {
        // Generate a unique tag 
        return Guid.NewGuid().ToString();
    } 
    
    // Get The File Extenstion
    public  static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("Invalid file name.");

        return Path.GetExtension(fileName).ToLower(); // Returns the file extension with a dot, e.g., ".jpg"
    }

    // Method For Debugging Purpose
    public static void LogObject(object obj, int indentLevel = 0)
    {
        if (obj == null)
        {
            Console.WriteLine($"{new string(' ', indentLevel)}null");
            return;
        }

        var type = obj.GetType();

        // If the object is a simple type, log it directly
        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
        {
            Console.WriteLine($"{new string(' ', indentLevel)}{obj}");
            return;
        }

        // If the object is a collection, iterate over its items
        if (obj is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                LogObject(item, indentLevel + 2);
            }
            return;
        }

        // Log each property of the object
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var propertyValue = property.GetValue(obj);
            Console.WriteLine($"{new string(' ', indentLevel)}{property.Name}:");

            // Recursively log the property value
            LogObject(propertyValue, indentLevel + 2);
        }
    }

}
