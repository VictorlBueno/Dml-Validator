<h6>❗This code is under development | Only the insert check is ready to use. </h6>

## DML Validator ☕
<p>The library provides functions to validate INSERT, DELETE, and UPDATE statements. When calling these functions and passing the Data Manipulation Language (DML) code as a parameter, each method will return true if the DML command is valid and false if it is not.
  
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#dml-validator-">What is DML Validator?</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#dml-validator-">How it Works?</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#syntax-validation">Syntax Validation</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#handling-nullundefinedna-contents">Handling Null, Undefined and #N/A Contents</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#matching-number-of-columnsvalues">Matching Number of Columns/Values</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#auto-identification-of-quotes">Auto Identification of Quotes</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#sql-injection-prevention">SQL Injection Prevention</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#ddl-validator">DDL Validator</a></li>
<li><a href="https://github.com/VictorlBueno/Check-Sql/tree/main#mass-validation">Mass validation</a></li>
<br>

For example, you can use the library to validate SQL queries before executing them in a database. If the DML code is well-formed and follows the syntax rules for the corresponding DML operation, the library will return true, indicating that the statement is valid and can be safely executed. On the other hand, if the DML code contains errors or doesn't adhere to the expected syntax, the library will return false, allowing you to handle the invalid statement appropriately in your application.</p>
<br>

## How it works
<p>
The function receives a command and identifies which command it corresponds to: INSERT, UPDATE, or DELETE, and points to the responsible function.

Then, it performs some analyses to identify the used DML pattern, such as whether column names exist or only values, the number of columns present, and whether there is a semicolon at the end. It saves this information in an object.

```CSharp
{ "hasColumnName", (sqlAsList[2].Contains("(") || sqlAsList[3].Contains("(")) },
{ "hasSemicolon", insertCommand[insertCommand.Length-1].ToString().Equals(";") },
...
```

After identifying these characteristics, it constructs a list with parts of a regex

```CSharp
// { "INSERT", 1 OR MORE SPACES, "INTO", 1 OR MORE SPACES, ALPHANUMERIC NAME, 0 OR MORE SPACES }
List<string> patternList = new List<string> { "INSERT", "\\s+", "INTO", "\\s+", "\\w+", "\\s*" };
```
And throughout the code, it identifies the remaining necessary patterns and adds them to this list in the expected order.</p>

```CSharp
...
// Add an alphanumeric column name and additional spaces
patternList.Add("\\w+\\s*");
```
<br>

## Syntax Validation
<p>The library validates the syntax of INSERT, DELETE, and UPDATE statements, ensuring they follow the correct structure.</p>

<p>For an automatic detection of the dml command and validate it:</p>

```CSharp
CheckDml.IsValid("INSERT INTO user (name, email) VALUES ('Victor', 'victor@example.com');") // true
CheckDml.IsValid("DELETE FROM user WHERE id = 1;") // true
CheckDml.IsValid("UPDATE user SET email = 'victorlb@example.com' WHERE id = 1;") // true
```
<p>You can also restrict validation to just a specific command:</p>

```CSharp
CheckDml.IsInsertValid("INSERT INTO user (name, email) VALUES ('Victor', 'victor@example.com');") // true 
CheckDml.IsDeleteValid("DELETE FROM user WHERE id = 1;") // true
CheckDml.IsUpdateValid("UPDATE user SET email = 'victorlb@example.com' WHERE id = 1;") // true
```
<p>Examples of syntax that will return false:</p>

```SQL
INERT INTO products (name, price) VALUES ('Hat', 100.00); # false
INSERT INT products (name, price) VALUES ('Coat', 165.99); # false
INSERT INTO pr@ducts (name, pri%ce) VALUES ('Legs', 120.00); # false
INSERT INTO products ('name', price) VALUES ('Boots', 230,00); # false
```

<p>Examples of syntax that will return true:</p>

```SQL
INSERT INTO products (name, price) VALUES ('Hood', 230.00); # true
INSERT INTO products(name, price)VALUES('Socks', 99) # true
INSERT INTO products(name, price)VALUES('Socks', 99) # true
INSERT INTO products VALUES('Socks', 99) # true
```
<br>

## Handling Null, Undefined and #N/A Contents
<p>The library handles null or undefined values gracefully, preventing potential errors when processing the DML code.</p>

```CSharp
CheckDml.IsValid("INSERT INTO products VALUES ('Hat', null)", "null") // false
CheckDml.IsValid("INSERT INTO products VALUES ('Hat', 100.00)", "null") // true
CheckDml.IsInsertValid("INSERT INTO products VALUES (undefined, 100.00)", "null") // false
CheckDml.IsInsertValid("INSERT INTO products VALUES (undefined, 100.00)", "null", "undefined", "#N/A") // false
```
<br>

## Matching Number of Columns/Values
<p>The library ensures that the number of columns matches the number of values in an INSERT statement, avoiding inconsistencies and errors.</p>

```SQL
INSERT INTO products (name, price) VALUES ('Hood'); # false
INSERT INTO products (name, price, size) VALUES ('Socks', 99); # false
INSERT INTO products VALUES ('Socks', 99); # true
```
<br>

## Auto Identification of Quotes
<p>The library automatically detects the use of double or single quotes, adapting its validation accordingly to support both types of strings.</p>

```SQL
INSERT INTO products VALUES ('Hat', 49.99) // true
INSERT INTO products VALUES ('Hat', 49.99, "blue") // false
INSERT INTO products VALUES ('Hat', 49.99, 'Totally "unbeatable" comfort') // true
```
<br>

## SQL Injection Prevention
<p>The library safeguards against SQL injection attacks, offering robust security measures to protect your database from malicious input.</p>

```CSharp
CheckDml.IsValid("INSERT INTO products(name, price)VALUES('Socks', 99); DROP TABLE products;") // false
CheckDml.IsDeleteValid("DELETE FROM user WHERE id = 1; DROP TABLE products;") // false
```
<br>

## DDL Validator
<p>This function is under development</p>
<br>

## Mass Validation
<p>At the end of this project, this code will be implemented in an interface created with Windows Forms for mass validation of SQL commands saved in <code>.xls</code> or <code>.txt</code>.</p>

#
<h6>Links&ensp;&ensp;&ensp;&ensp;
<a href="https://linkedin.com/in/victorlbueno/" target="_blank">LinkedIn</a>&ensp;&ensp;•&ensp;&ensp;
<a href="https://victor.com.de/" target="_blank">Website</a>&ensp;&ensp;•&ensp;&ensp;
<a href="https://instagram.com/victorlbueno" target="_blank">Instagram</a></h6>
