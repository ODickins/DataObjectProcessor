# DataObjectProcessor


## What is this package?
DataObjectProcessor is a basic data access package to make it easier to work with differing data storage techniques.
Designed initially to allow the quick processing of CSV files into a List of Generic Types but contrained to an interface
so that other data storage techniques can be added later.

## How do I use this package?
1. Install the package using Nuget into your project.

2. Use the package from the namespace 'DataObjectProcessor'
```
/Class1.cs

using DataObjectProcessor;
```
3. Add the Data Processor of your choosing.
```
/Class1.cs

IDataProcessor dp = new DataObjectProcessor.CsvProcessor.CsvDataProcessor(@"C:\MyCsvFile.csv");

```
4. Itterate down the file by using one of the methods outlined in the interface.
```
/Class1.cs

await foreach (var i in dp.GetAsyncEnumerable<MyClass>()){
    Console.WriteLine(i.MyProperty);
}

class MyClass {
    public string MyProperty { get; set; }
}

```

## Special Considerations
There are a few ways to customize the packages usage.
-----------------------------------------------------------
1. Change the options used by the CsvProcessor
```
/Class1.cs

IDataProcessor dp = new CsvDataProcessor(@"C:\MyCsvFile.csv", new CsvProcessorOptions()
            {
                ColumnSeperator = ",",
                FieldsAreEnclosedInQuotes = true,
                FirstRowHasHeadings = true,
                ThrowWhenUnableToSetPropertyValue = true
            });
```
2. Setup Csv Column Index's to match properties to columns with different names
```
class MyClass {
    [CsvIndex(1)]
    public string MyProperty { get; set; }

    [CsvIndex(2)]
    public string SecondProperty { get; set; }

    public string Another { get; set; }
}
```

2.1 Example CSV File (For above)
```
Another,Firstname,Lastname
123,"Wayne","Johnson"
```
In the above example, '123' would be mapped to 'Another as the column header matches the property name, "Wayne" would be mapped to "MyProperty" as it's Index in the Csv file is '1', and "Johnson" would be mapped to "SecondProperty"

Exceptions.
----------------------
1. 'PropertyConversionException' will be thrown in the data inside the Csv file is unable to be matched to the property. You can set 'ThrowWhenUnableToSetPropertyValue' to false, thus - when the data cannot be converted to the property type, it will silently fail and leave the property unpopulated.

2. 'PropertyIndexException' will be thrown if you set the same column index on two properties. You can only have one property mapped to one column.

---

#### Tips
More underlying data storage techniques will be added in the future, as and when I personal need them.
If you find any tools within here helpful, consider buying me a beer - I'd appreciate it.

[Use PayPal to buy me a beer.](https://www.paypal.com/paypalme/odickins)


##### Change Log

###### 1.0.0 -> Initial Publish
###### 1.0.1 -> Change to make it slightly more reliable. Was missing some properties.