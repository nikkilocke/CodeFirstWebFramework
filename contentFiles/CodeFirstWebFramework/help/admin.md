# Admin

## Settings

This shows the program and database version numbers, and allows you to select the UI skin. 

You can add new skins by creating a pair of css and js files in the skins folder - these are both loaded
after the standard css and js files, and may change the appearance however required.

## Users

If there are no users on file, there is no login security. Once you add a user, some or all functionality will 
require logging in as a user. The first user you create will be the Admin user, who has access to everything. 
You cannot delete this user until all the other users have been deleted (and deleting this user will turn off 
login security).

All other users have an Access Level, which can be set to None, Read Only, Read Write or Admin. Any functionality
which requires login requires one of these access levels.

You can also gain finer control over who has access to what by ticking Module Permissions. This will show a list of
all the modules and/or methods which require an access level, and you can set the user's level for each one individually.

## Backup and Restore

Backup backs up your database to a json file, and downloads it so you can store it on your computer (or email it to someone).

Restore restores the database from a backup file - **this will overwrite all your data with the old data**.

## Login/Logout

If security is on (there are users in the database), you will be offered the option to login and logout here.

