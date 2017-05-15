# CodeFirstWebFramework Web App development tool

## Introduction

This DLL enables you to build a highly functional C# web app with minimal coding. Your Main method has only to call
    Config.Load(args);
    new WebServer().Start();
This will load (or create) the config file, analyse the code for classes which should be persisted to the database, ensure the database schema is up to date with the code, and start listening for web requests on the default port (8080).

The database is a local SQLite database by default, but can be set (in the config file) to any accessable MySql or Sql Server database. The default port and a number of other settings can also be overridden in the config file.

This starts the web server which will server any html or other files placed in a subfolder with the same name as the main namespace of your project.

However, this is only the beginning. Instead of serving html files, the server can construct the html by merging tmpl files with the default.tmpl file. This allows you to make the default.tmpl file a standard look and feel for the whole website, with individual page data being inserted in it as appropriate.

Even more useful, selected pages can be created or altered using C# code - each C# class which is a subclass of AppModule acts as a virtual directory, and each public method in that class acts as a virtual file in that directory. So, for example. a web request for /home/listing.html will create the AppModule subclass Home, and call the Listing method. This method could retrieve a list of records from the database, and the home/listing.tmpl file could display them as a searchable table.


