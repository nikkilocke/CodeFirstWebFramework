# CodeFirstWebFramework Documentation

## [Api documentation link](Api.md)

## Building a project which uses CodeFirstWebFramework

Download the latest NuGet `CodeFirstWebFramework` package using the NuGet package manager.

CodeFirstWebFramework provides a number of template, javascript, css, etc. files in the `contentFiles/CodeFirstWebFramework` folder. You need to copy this folder to your program exe folder - I use a post-build command:

	xcopy /E /I /Y "$(ProjectDir)contentFiles\CodeFirstWebFramework" "$(TargetDir)CodeFirstWebFramework"

## Building CodeFirstWebFramework itself

If you want to build and/or modify the framework, you will require Visual Studio 2017 or later, as the project file depends on automatic build features introduced in that version.

You can convert the project to an earlier version of Visual Studio, or build it with Mono - to do that you just need to add all the .cs files to the project (whereas Visual Studio 2017 searches the directory automatically), and add all the required NuGet packages listed in the existing `.csproj` file.

## Sample projects which use CodeFirstWebFramework

A simple phone number database project is provided with the source, which demonstrates some of the principles, and uses most of the automatic behaviour built in to CodeFirstWebFramework.

The [AccountServer](https://github.com/nikkilocke/AccountServer) github project is a much more complex project, which uses templates extensively (it was originally written before the Form classes existed, so some of the simpler templates could be replaced by Forms), and overrides Database, Settings, Admin and AccessLevel.

I would be pleased to hear of any other projects which use it - you can email me from my website at [http://www.trumphurst.com/contact.php](http://www.trumphurst.com/contact.php).

## Config files
 
The first time you run a program that calls `Config.Load`, if there is no existing Config file with the same name as the program (but a .config extension), it will create one with the default values, which you can then edit. Note that the config file is loaded when the program runs - if you change it, you have to restart the program for the new values to take effect. 

The Config file lives in the Common Application Data folder - `C:\ProgramData\AccountServer` in Windows, `/usr/share/AccountServer` in Linux.
 
The Config file has the following main variables
 
|Variable|Description|Default|
|-----------|---------------|---------|
|Database|The type of database - SQLite, MySql, or SQLServer|SQLite|
|ConnectionString|For the type of database|a .db file with the same name as the program, in the user’s data area|
|Namespace|The C# Namespace in which to search for modules and methods to call for a request page|The Namespace of the entry assembly|
|Port|The port the web server listens on|8080|
|ServerName|The Url used to access the server|localhost|
|Email|Email used as from address when sending emails|root@localhost|
|SessionExpiryMinutes|Number of minutes of inactivity before discarding session data|30|
 
The web server can actually handle requests for more than 1 web app. Each web app must be in its own C# Namespace. The server distinguishes which web app/Namespace to call from the server part of the Url (including the port). To enable this, add an entry for each app to the `Servers` array in the config file. Each Servers entry can have the following variables:
 
|Variable|Description|
|-----------|---------------|
|Database|The type of database - SQLite, MySql, or SQLServer|
|ConnectionString|For the type of database|
|Namespace|The C# Namespace in which to search for modules and methods to call for a request page|
|AdditionalAssemblies|Array of additional assemblies (DLL or EXE) to load into the program. This facility allows you to add additional web apps from separate files, and have them all run in your single web server.|
|ServerName|The Url used to access the app|
|ServerAlias|Other Urls for this app, separated by spaces. You can use an asterisk (\*) wildcard in the ServerAlias (e.g. `*.trumphurst.com`).|
|Port|The port the web server listens on|
|Email|Email used as from address when sending emails|
|Title|Default app name to use in web page title|
 
The default values for these variables are the values from the config file as above.

Example config file for 2 web apps running on different ports on localhost:

	{
	  "Database": "SQLite",
	  "Namespace": "Phone",
	  "ConnectionString": "Data Source=C:/ProgramData/Phone/Phone.db",
	  "Port": 8080,
	  "SlowQuery": 100,
	  "ServerName": "localhost",
	  "Email": "root@localhost",
	  "SessionExpiryMinutes": 30,
	  "Servers": [],
	  "SessionLogging": false,
	  "DatabaseLogging": 0,
	  "PostLogging": false,
	  "Servers": [
			{
			"ServerName": "localhost",
			"Port": 8080,
			"Namespace": "Phone",
			"Title": "Phone Book",
			"Database": "SQLite",
			"ConnectionString": "Data Source=C:/ProgramData/Phone/Phone.db"
			},
			{
			"ServerName": "localhost",
			"Port": 8081,
			"Namespace": "AddressBook",
			"Title": "Address Book",
			"Database": "SQLite",
			"ConnectionString": "Data Source=C:/ProgramData/AddressBook/AddressBook.db"
			}
	  ]
	}

Example config file for 1 web app running on 2 different hostnames with the same web app but different databases:

	{
	  "Database": "SQLite",
	  "Namespace": "Phone",
	  "ConnectionString": "Data Source=C:/ProgramData/Phone/Phone.db",
	  "Port": 8080,
	  "SlowQuery": 100,
	  "ServerName": "localhost",
	  "Email": "root@localhost",
	  "SessionExpiryMinutes": 30,
	  "Servers": [],
	  "SessionLogging": false,
	  "DatabaseLogging": 0,
	  "PostLogging": false,
	  "Servers": [
			{
			"ServerName": "localhost",
			"Namespace": "Phone",
			"Title": "Phone Book (business)",
			"Database": "SQLite",
			"ConnectionString": "Data Source=C:/ProgramData/Phone/Phone.db"
			},
			{
			"ServerName": "127.0.0.1",
			"Namespace": "Phone",
			"Title": "Phone Book (personal)",
			"Database": "SQLite",
			"ConnectionString": "Data Source=C:/ProgramData/Phone/Personal.db"
			}
	  ]
	}


## Directory searching

CodeFirstWebFramework provides a number of template, javascript, css, etc. files in the `contentFiles/CodeFirstWebFramework` folder. You need to copy this folder to your program exe folder - I use a post-build command:

	xcopy /E /I /Y "$(ProjectDir)contentFiles\CodeFirstWebFramework" "$(TargetDir)CodeFirstWebFramework"

You can provide alternative versions of any of these files in a folder with the same name as your program. If you have more than 1 web app, and you want different versions for each one, you can provide them in a folder with the same name as the server (e.g. `localhost`).

Finally, if your users want to override any of these files for their individual installation (without editing your versions, which would be overwritten during an upgrade), they can provide versions in any of the above folders, but in the Common Application Data/Program Name folder.

So whenever the web server is looking for a file (e.g. an html file, or a template tmpl file, or a js, css, or jpg file), it searches through the following set of directories:

* Common Application Data/Program Name/Server Name
* Installation Directory/Server Name
* Common Application Data/Program Name/Namespace
* Installation Directory/Namespace
* Common Application Data/Program Name/Default Server Name
* Installation Directory/Default Server Name
* Common Application Data/Program Name/`CodeFirstWebFramework`
* Installation Directory/`CodeFirstWebFramework`

In the above:

|Name|Location|
|----|--------|
|Common Application Data|`C:\Program Data` in Windows, `/usr/share` in Linux|
|Program Name|The name of the program exe file|
|Installation Directory|Where the program is installed - e.g. `C:\Program Files\ProgramName`|
|Server Name|The ServerName from the Config file Servers array|
|Namespace|The Namespace from the Config file Servers array|
|Default Server Name|The Namespace from the Config file main section|

So, for example, if the program is called Phone, Namespace Phone, and the web request is for http://localhost:8080/scripts/default.js, the search order is

* C:\Program Data\Phone\localhost\scripts\default.js
* C:\Program Files\Phone\localhost\scripts\default.js
* C:\Program Data\Phone\Phone\scripts\default.js
* C:\Program Files\Phone\Phone\scripts\default.js
* C:\Program Data\Phone\CodeFirstWebFramework\scripts\default.js
* C:\Program Files\Phone\CodeFirstWebFramework\scripts\default.js


## Mustache templates

A web request for an html file will search the above folders for it. If it is not found, it will search again for a .tmpl file with the same name. Tmpl files are Mustache templates. For full documentation of Mustache templates see [https://github.com/jehugaleahsa/mustache-sharp](https://github.com/jehugaleahsa/mustache-sharp). Basically, you can insert placeholders of the form `{{PropertyName}}` into the text, and they will be replaced by the value of that property in the AppModule that is running. Examples of properties provided in AppModule include:

|Variable|Description|
|--------|-----------|
|Settings|The Settings record read from the database|
|Session|The Session|
|Server|The Server details from the Config file|
|Request|The HttpListenerRequest|
|Title|Web page title|
|Config|The Config file|
|Today|Today's date in yyyy-MM-dd format|

Of course, if you write your own subclasses of AppModule, their properties will also be available.

All .tmpl files are rendered into a string, which is then searched for xml elements called `title`, `head` and `body`. If these exist, their contents are copied into AppModule fields `Title`, `Head` and `Body`. The result is then used to render `default.tmpl`, which gives the final html page returned to the request. This means that you can set up your website look and feel in default.tmpl, and then just change the inner content in individual template files.

Note that you can add additional string fields to your own AppModules, marked with the `[TemplateSection]` attribute, and these will be treated the same as title, head and body above.

See [CodeFirstWebFramework/admin/batch.tmpl](CodeFirstWebFramework/admin/batch.tmpl) and [CodeFirstWebFramework/default.tmpl](CodeFirstWebFramework/default.tmpl) for examples.

Note that CodeFirstWebFramework Mustache templates preserve newlines in templates, and provide a number of enhancements to standard Mustache-Sharp, as follows:

|Markup|Function|
|------|--------|
|{{include *filename*}}|Searches for *filename* in the search folders, and reads it in.|
|{{{*variable*}}}|Having replaced *variable*, html quotes it, so that html special characters are rendered as expected. For example if the variable contains &lt;B&gt;, it appears in the rendered html page as is, instead of turning on bold print.|
|// {{*mustache*}}|Is replaced by {{*mustache*}}. This is so you can hide mustache in a comment in javascript files to avoid apparent syntax errors.|
|'!{{*mustache*}}'|Is replaced by {{*mustache*}}. This is so you can hide mustache in a string in javascript files to avoid apparent syntax errors.|

## Database class

The provided Database class provides extensive methods for reading objects from the database, updating and inserting, transactions, etc. It has a built-in Upgrade method that will find all the tables and views defined in the C# code, and create or update all the table structures in the database to match.

As your program evolves, you may find it necessary to run additional code when upgrading the database from one version to another (e.g. to fill in new fields with specific values, or maybe you are splitting a table into 2 related tables). To cater for this, there is a DbVersion field in the Settings table, which is automatically compared with the CurrentDbVersion virtual property in the Database class. You can create a subclass of Database for each of your Namespaces (web apps), and override the `CurrentDbVersion` property, and the `PreUpgradeFromVersion` and `PostUpgradeFromVersion` virtual methods to run any such code. The `PreUpgradeFromVersion` is run before any changes are made to the database structure, and `PostUpgradeFromVersion` is run after.

### Database tables

Database tables are declared as C# classes, which must be subclasses of `JsonObject`, and have the `[Table]` attribute. Most tables will have a unique, auto-increment integer field as the primary key - this should be given the `[Primary]` attribute. Some of the javascript code assumes by default that the primary key is called `idTableName` (where TableName is the class name), although there are facilities to change this in forms. This field is referred to as the record id below. 

In order to make SQL statements joining tables easier to write, it is often preferable to make your field names globally unique. To make this easier, the framework understands that variable names are often preceded by their table name (e.g. in the Document table, a field called DocumentName), and it will strip off the table name when displaying headings and prompts to the user.

In the table, you can add other attributes to fields to map them to the database correctly:

|Attribute|Description|
|---------|-----------|
|`Unique(string name, int order = 0)`|This field (or combination of fields) is a unique key. For a combination of fields, use the Unique attribute with the same name, and sequential orders.|
|`Nullable`|Indicates the field may be null.|
|`Length(int length, int precision = 0)`|Indicates the length of the field - for decimal fields, use 2 parameters for length and precision. A string field with length explicitly set to 0 will be stored as a variable-length memo field - otherwise the default is 45 for a string field, 10.2 for decimal, 10.4 for double, 11 for integer and 1 for bool.|
|`DefaultValue(string value)`|The default value used if a record is created with no value for this field.|
|`ForeignKey(string table)`|This field is an integer foreign key which refers to the record id of the given table.|
|`DoNotStore`|This field exists in the class, but is not stored in the database.|

You can also create classes which are mapped to views on the database - use the `View(string sql)` attribute - `sql` is the SQL to query the database and produce the view.

### Retrieving and updating data

Data is typically retrieved using the `Query`, `QueryOne` and Get methods, each of which has multiple overrides. `Query` always returns an Enumerable with a series of records, and `QueryOne` and Get return a single record (or, possibly, null).

Query can accept a SQL string which should return the records - these are returned as JObjectEnumerable. There is a templated override Query<Type> which returns an IEnumerable<Type>, so, for example, if you have a class called `Document` you could iterate through all the Documents with:

    foreach(Document d in Database.Query<Document>("SELECT * FROM Document"))

The Get methods return the record with a particular id. There is also an override which takes a partly filled in JObject or C# class object, and returns the record whose primary or unique keys match it.

To help you build queries, there is a `Quote` method which accepts any type, and quotes it appropriately for the type of database you are using.

The `In` methods make is easy to build correctly quoted `IN(...)` SQL statements.

The `Cast` method lets you cast one type to another in a query in whatever syntax applies to your particular database.

You can update, insert and delete data with the `Update`, `Insert` and `Delete` methods, which will accept a C# class object, or a JObject and a table name. If you call `Update` on a record which has a null or zero record id, a new record will be inserted, and the record id in the passed object updated to reflect the inserted value.

You can also execute arbitrary SQL statements with the `Execute` method.

## AppModule class

Every Namespace which implements a web app must have at least one class derived from AppModule. Each such class contains code to implement one part of the web app, and appears to the web browser to be a folder. The AppModule will implement one or more public methods, which will appear to the web browser to be html files in the folder. The base AppModule has a virtual `Default` method, which is the one called if no file is specified - like index.html in the Apache web server.

The WebServer class will determine which class is required, and then creates an object of that class, and calls its `Call` method. This collects the parameters, and calls CallMethod to find the correct method and call it.

### Method parameters and returns

The simplest method has no parameters, and no return value. Once it has been called (by a web request for `/class/method.html`), on its return the Call method will see that no response has been sent yet, and call Respond, which looks for a template file with the same name (i.e. /class/method.tmpl), fills in that template using the AppModule, and returns the resulting html. Any parameters provided in the request are supplied in the `AppModule.GetParameters` and `AppModule.PostParameters` NameValueCollections. There is also an `AppModule.Parameters` NameValueCollection which is a merge of both of the above.

A method may also have named parameters. CallMethod will look for request parameters with the same names, and fill in the parameters to the method accordingly. If the parameters don't match, or are not all supplied, it throws an Exception, which will  will return a 500 Internal Server error (along with details of the exception formatted with exception.tmpl).

A method may also return something - if it returns a Form, the form is rendered. Otherwise the Call method will call WriteResponse to convert the returned type to a response. Streams and strings are returned unchanged - any other non-null return value is converted to a json object. The javascript provided often uses Ajax calls (e.g. to save a form), and many of these expect an object of type `AjaxReturn` to return the status and results of a call.

A method may instead call WriteResponse or Respond itself to send back results, if the default behaviour is not sufficient. Or even write the result itself - in which case it should set ResponseSent to true, to prevent the default processing.

### Caching and versioning

All pages delivered from an AppModule are set to disable web browser caching by default - it is assumed that most code-driven pages may be different on every call. You can allow caching by setting CacheAllowed during the call.

By default web browsers will cache apparently static files - like `default.css` and `default.js`, for example, which are loaded as script files from `default.tmpl`. This becomes an issue when a new version of your app is installed, and the file has changed, but the web server does not bother to load it. While testing, you can get round this with a forced refresh of your browser, but you cannot expect end users to know about this. So there is a built-in mechanism for defeating the caching - append `{{VersionSuffix}}` to the filename in your template file. This suffix includes the program version number, so when you change the version number, the web browser will think it is loading a different filename, and not use its cache. The suffix is automatically removed from the filename by the web server before looking for the file on disk.

### Batch (long running) code

If the code in response to a web request is going to run for longer than a second or so, you should take advantage of the built-in batch running facilities, to give the user feedback on the progress of the code. See the `Phone` test app `ImportSave` method for an example.

At the start of your method, create a `BatchJob`. The constructor takes a `delegate` or `Action` containing code to run the job. `CodeFirstWebFramework` will automatically redirect the browser to `/admin/batchstatus`, which shows the progress of the batch, and redirects back to the original page (or another page of your choice) when the batch is finished.

Inside the delegate, the `Batch` property contains the BatchJob instance. `BatchJob` has the following properties to help the job communicate with the user:

|Property|Usage|
|--------|-----|
|Records|The number of records the batch will process, for the progress bar.|
|Record|The current record being processed, for the progress bar.|
|Status|A string describing what is happening now.|

Any exceptions inside the delegate will be caught, logged, and the message displayed to the user.


## Default.js

The provided default.js javascript file (which is pulled in by default.tmpl) has lots of built-in behaviour to make writing web page templates simpler. For example, buttons with an `href` attribute act like links, and buttons with a `data-goto` attribute act like links, but record the page state, so a button with the `cancel` class will return to that state. See the source code for more information.

## Forms

Default.js also provides the methods necessary to implement ajax-enabled forms. You can create these forms by hand in javascript, but there are C# classes to enable you to create them in C# code with the minimum of coding. There are 4 types of form.

The C# forms are all based on the Form class. You can create a form to view or input a C# class in the constructor, or by calling by calling `Build(Type)`. This will add the necessary columns to the form. By default the types of each field are deduced from the members of the class, as modified by the attributes described under Database tables above. E.g. a ForeignKey field will appear as a select dropdown containing the records from the other table. By default, fields in a Table class will be read-write, and extra fields in a derived or other class (e.g. a View) will be read only.

You can also include Properties (rather than Fields) in your form, or determine which specific fields you want, and their order in the form if you construct the form by providing a specific list of fields/properties.

You can further modify this behaviour with the following attributes:

|Attribute|Description|
|---------|-----------|
|ReadOnly|Makes the field read only|
|Writeable|Makes the field writeable, even if it would be read only by default|

You can also apply the Field attribute, which has the following fields which may be set to modify the behaviour:

|Field|Description|
|-----|-----------|
|Data|The name of the field containing the data.|
|Name|The name of the field - default is the same as Data.|
|Type|The kind of field to use - see default.js Type object for a full list of types.|
|Heading|The heading or prompt for the field.|
|Colspan|In input forms, gives the field a colspan so it takes more columns.|
|SameRow|In input fields, puts this field on the same row as the previous one.|
|Attributes|Html attributes added to the field.|
|Size|The size/length of the field - by default comes from the Length attribute.|
|Visible|Set to false to hide the field (but it is still searchable).|

For very fine control, you can set any of the other properties supported in the javascript by supplying property name, value pairs to the constructor, or by adding these properties to the field's `Options` JObject later in the code (the Form's `this` property can return the FieldAttribute for any field by name).

You can make a field into a select drop-down by calling `MakeSelect(IEnumerable<JObject> values)`. Values is an enumerable list of JObjects which contain as a minimum an id (to place in the field when returning the value) and a value (to display to the user).

All the Form classes have a field called `Data`, which may be set to a JToken containing the form data - if none is supplied, the DataTable and List forms will make an Ajax callback to get the data to the current method with `Listing` added to the end.

They also have a field called Options which is a JObject to which you can add other attributes supported by the javascript.
These include:

|Property|Description|
|--------|-----------|
|table|The name of the table - used to strip off table prefixes from variable names.|
|id|The name of the [Primary] id field of the table - default is "id" + the table name.|

You can display the form in the web page by calling `Form.Show()`, or by returning the Form from your AppModule method.

### DataTable

The DataTableForm class creates a jquery datatable - this is a table which is a window on a dataset, enabling the user to scroll, sort and search the data and, optionally, to select a record for further processing.

There is a Select property, which can be set to a url to call when the user selects a record from a list. If a url, an id parameter will be added, containing the id field from the selected record.

There is the built-in ability to filter out records with zero values - if you set a field's "nonZero" option to either True or False, the javascript will add a button to show or hide zero values (if you set it to True, zero values are hidden to start with, and vice versa).

### Form

The Form class creates an input or display form. For input forms, "Save" and "Save and Close" buttons are provided, which will include the amended form data as a json object called `json` in the post parameters to the current method with `Save` added to the end. This expects an AjaxReturn object to be returned indicating success or failure, and including the Primary id of any newly-created record. You can override this url by adding a `submit` property to Options.

If a `canDelete` property is added to Options, a Delete button will be provided, which calls the current method with `Delete` added to the end, providing the record id as a parameter. You can override this url by adding a `delete` property to Options.

### ListForm

This creates a possibly editable editable list of records. 

You can allow the user to select an item from the list by setting the Select property to a url to call with the record id in an `id` parameter. If you do not do so, Save buttons will be provided, which will include the amended form data as a json object called `json` in the post parameters to the current method with `Save` added to the end. This expects an AjaxReturn object to be returned indicating success or failure, and including the Primary id of any newly-created record. You can override this url by adding a `submit` property to Options.

### HeaderDetailForm

This is a header Form followed by a ListForm containing lines for that header. The Data provided must contain a header property containing the header data, and a detail enumerable containing the detail lines.

### Form Save and Delete methods, and the AjaxReturn class

Form Save and Delete methods return an `AjaxReturn` object to the calling javascript. This contains fields for communicating with the javascript (in lower case, following javascript conventions). These are:

|Field|Purpose|
|-----|-------|
|error|If an exception occurs in a method which returns an `AjaxReturn`, it is trapped automatically, and the message placed here. Or you can set this yourself to indicate a reason the call has failed.|
|message|This message is displayed at the top of the form.|
|redirect|This instructs the javascript to redirect to a different page.|
|confirm|Ask the user to confirm something (with this prompt), and resubmit with confirm parameter if the user says yes.|
|id|For a Save method, this should be set to the id of a newly saved record. If set, the javascript will reload this record into the form.|
|data|Arbitrary data to send to the javascript. For instance, `batchstatus` uses this to get the batch progress data.|

Note that `AppModule` provides `SaveRecord` and `DeleteRecord` methods which will save or delete a record and return a suitably filled in AjaxReturn. For simple single record updates you can do your validation on the supplied data, then return the result of `SaveRecord`. Note that the `Utils.Check` method (which checks its first argument is true, and thors an exception with the supplied message if not) is a useful shorthand for validation checks.

## DumbForm

This creates an ordinary html form, with the Save button submitting the form, by default to the original method name with `Save` added to the end (you can alter this with the `submit` property).

## Help

CodeFirstWebFramework includes the built-in ability to display Markdown (.md) files, and these are used by the help system. You should write a `help/default.md` file containing the markdown for the help table of contents. This file should have headings which consist of links to the various help files e.g.

	# Using the AccountServer accounting system
	
	## [Installation](installation.md)
	
	### [Importing data from other systems](admin_import.md)
	
	## [Navigation - how the screens work](navigation.md)

It can also contain any other markdown you wish. The C# Help AppModule will use this file to work out the structure of the help system, so it can generate Next, Previous and Up links.

The files referred to can be linked to anywhere (e.g. `/help/installation.md`). When any of the md files in the help folder are displayed, they are automatically included in `/help/default.tmpl`, which implements the Table of Contents, Next, Previous and Up links. You can even template a help file itself - instead of creating a .md file, create a .tmpl file of the same name, and you can include Mustache in the file. This would probably be most useful if you have your own Help class (derived from CodeFirstWebFramework.Help) with its own variables to use in the template.

The help becomes context sensitive, because every AppModule has a Help property, which will automatically contain a link to any file file in the help folder with the name *module*_*method*.md, or, if that doesn't exist, the name *module*.md. You can add this link to your `/default.tmpl` file to provide a context-sensitive help link. Note it will be an empty string if there is no such file, which you can test for with code like `{{#if Help}}<a href="{{Help}}">Help</a>{{/if}}`.

## Login and security

By default, there is no user login or security. However, if you provide an interface to edit users, and to login (normally in the `Admin` module - see the Admin module provided) and at least one user is added, logging in will be enabled.

You can limit access to a particular module, or to a particular method in a module by adding an `[Auth]` property. Doing so automatically adds that module or method to the list of modules and methods which require a particular access level. You can specify the access level in the `Auth` constructor (any integer, but pre-defined values are provided as constants in the `AccessLevel` class), or you can write code to check the user's access level (in `AppModule.UserAccessLevel` - this is automatically set to `Admin` if security is disabled because there are no users).

If no access level is specified for a method, it inherits the access level of the module. By default, if the access level of a method is `ReadOnly`, then the corresponding `Save` and `Delete` methods will be `ReadWrite` - there is no need to specify those separately. If most of your methods require the same access level, but one can be accessed by anyone (e.g. `Admin.Login`), you can set the access level on the module, and add an `Auth` parameter to the `Login` method with level set to `AccessLevel.Any`.

If there are no users on file, there is no login security. Once you add a user, all functionality marked with an `Auth` attribute requires logging in as a user. The first user you create will be the Admin user, who has access to everything. You cannot delete this user until all the other users have been deleted (and deleting this user will turn off login security).

All other users have an `AccessLevel`, which can be set to `None`, `ReadOnly`, `ReadWrite` or `Admin`. Any functionality which requires login requires one of these access levels. If you require additional levels in your application, create a new class derived from `AccessLevel`, with constants for the new levels. Any distinct values will be included automatically in the drop-down list of levels, or you can override the `Select` method to provide the data for the list if the default implementation is not sufficient. Note that the core code assumes `ReadOnly`, `ReadWrite` and `Admin` levels remain with their specific values, but space has been left to add intermediate values.

The user can also gain finer control over who has access to what by ticking `Module Permissions`. This will show a list of all the modules and/or methods which require an access level, and you can set the user's level for each one individually. By default, this shows every module and method with an `Auth` attribute, but you can remove some of them from the display by setting `Hide` on the attribute to `true` (the permission used will be that on the module), or show multiple `Auth` method attributes as a single row by setting a `Name` on the attribute - all attributes with the same `Name` are shown as a single row. If you don't specify a `Name`, it is derived from the method name by un-camel-casing the name.

