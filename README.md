# CodeFirstWebFramework Web App development tool

Now available as a NuGet package, downloadable directly from Visual Studio.

This DLL enables you to build a highly functional C# web app with minimal coding. Your Main method has only to call

    Config.Load(args);
    new WebServer().Start();

This will load (or create) the config file, analyse the code for classes which should be persisted to the database, ensure the database schema is up to date with the code, and start listening for web requests on the default port (8080).

The database is a local SQLite database by default, but can be set (in the config file) to any accessable MySql or Sql Server database. The default port and a number of other settings can also be overridden in the config file.

This starts the web server which will server any html or other files placed in a subfolder with the same name as the main namespace of your project.

However, this is only the beginning. Instead of serving html files, the server can construct the html by merging Mustache .tmpl files with the default.tmpl file. This allows you to make the default.tmpl file a standard look and feel for the whole website, with individual page data being inserted in it as appropriate.

Even more useful, selected pages can be created or altered using C# code - each C# class which is a subclass of AppModule acts as a virtual directory, and each public method in that class acts as a virtual file in that directory. So, for example, a web request for /home/listing.html will create an object of the AppModule subclass Home, and call the Listing method. This method could retrieve a list of records from the database, and the home/listing.tmpl file could display them as a searchable table.

Because the templates are Mustache, you can insert information from variables in the C# program directly into the output html, including processing lists and arrays.

There is an extensive library of javascript (in `default.js`) which works with the C# code to provide support for forms, data tables, array list/edit forms and header-detail forms, along with callbacks to the C# code using ajax and json.

There is built-in support for backing up and restoring the database (to json format).

There is built in (but extensible) optional support for creating users with logins and passwords, and giving each user a permission level for the whole system, or even individual modules or methods.

There is also built-in support for GitHub style Markdown (like this file), both in general use, and for writing context-sensitive help with a table of contents.

The DLL can actually run multiple web servers on the same port (distinguished by the server part of the url), or different ports, each of which can use the same or different C# code (distinguished by C# Namespaces) and use different databases. You can also customise the look and feel of each server, as the template code looks for templates in a folder named for the server, then a folder named for the app namespace, then in the CodeFirstWebFramework folder.

[More documentation is in contentFiles/Documentation.md](contentFiles/Documentation.md)

[Api documentation is in contentFiles/Api.md](contentFiles/Api.md)