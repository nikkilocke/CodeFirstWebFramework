<a name='assembly'></a>
# CodeFirstWebFramework

## Contents

- [AccessLevel](#T-CodeFirstWebFramework-AccessLevel 'CodeFirstWebFramework.AccessLevel')
  - [Admin](#F-CodeFirstWebFramework-AccessLevel-Admin 'CodeFirstWebFramework.AccessLevel.Admin')
  - [Any](#F-CodeFirstWebFramework-AccessLevel-Any 'CodeFirstWebFramework.AccessLevel.Any')
  - [None](#F-CodeFirstWebFramework-AccessLevel-None 'CodeFirstWebFramework.AccessLevel.None')
  - [ReadOnly](#F-CodeFirstWebFramework-AccessLevel-ReadOnly 'CodeFirstWebFramework.AccessLevel.ReadOnly')
  - [ReadWrite](#F-CodeFirstWebFramework-AccessLevel-ReadWrite 'CodeFirstWebFramework.AccessLevel.ReadWrite')
  - [Unspecified](#F-CodeFirstWebFramework-AccessLevel-Unspecified 'CodeFirstWebFramework.AccessLevel.Unspecified')
  - [Select()](#M-CodeFirstWebFramework-AccessLevel-Select 'CodeFirstWebFramework.AccessLevel.Select')
- [AdminHelper](#T-CodeFirstWebFramework-AdminHelper 'CodeFirstWebFramework.AdminHelper')
  - [#ctor()](#M-CodeFirstWebFramework-AdminHelper-#ctor-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.AdminHelper.#ctor(CodeFirstWebFramework.AppModule)')
  - [Backup()](#M-CodeFirstWebFramework-AdminHelper-Backup 'CodeFirstWebFramework.AdminHelper.Backup')
  - [Batch()](#M-CodeFirstWebFramework-AdminHelper-Batch 'CodeFirstWebFramework.AdminHelper.Batch')
  - [BatchJobs()](#M-CodeFirstWebFramework-AdminHelper-BatchJobs 'CodeFirstWebFramework.AdminHelper.BatchJobs')
  - [BatchStatus()](#M-CodeFirstWebFramework-AdminHelper-BatchStatus-System-Int32- 'CodeFirstWebFramework.AdminHelper.BatchStatus(System.Int32)')
  - [ChangePassword()](#M-CodeFirstWebFramework-AdminHelper-ChangePassword 'CodeFirstWebFramework.AdminHelper.ChangePassword')
  - [ChangePasswordSave()](#M-CodeFirstWebFramework-AdminHelper-ChangePasswordSave-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.AdminHelper.ChangePasswordSave(Newtonsoft.Json.Linq.JObject)')
  - [EditSettings()](#M-CodeFirstWebFramework-AdminHelper-EditSettings 'CodeFirstWebFramework.AdminHelper.EditSettings')
  - [EditSettingsSave()](#M-CodeFirstWebFramework-AdminHelper-EditSettingsSave-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.AdminHelper.EditSettingsSave(Newtonsoft.Json.Linq.JObject)')
  - [EditUser()](#M-CodeFirstWebFramework-AdminHelper-EditUser-System-Int32- 'CodeFirstWebFramework.AdminHelper.EditUser(System.Int32)')
  - [EditUserDelete()](#M-CodeFirstWebFramework-AdminHelper-EditUserDelete-System-Int32- 'CodeFirstWebFramework.AdminHelper.EditUserDelete(System.Int32)')
  - [EditUserSave()](#M-CodeFirstWebFramework-AdminHelper-EditUserSave-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.AdminHelper.EditUserSave(Newtonsoft.Json.Linq.JObject)')
  - [Login()](#M-CodeFirstWebFramework-AdminHelper-Login 'CodeFirstWebFramework.AdminHelper.Login')
  - [Logout()](#M-CodeFirstWebFramework-AdminHelper-Logout 'CodeFirstWebFramework.AdminHelper.Logout')
  - [Restore()](#M-CodeFirstWebFramework-AdminHelper-Restore 'CodeFirstWebFramework.AdminHelper.Restore')
  - [Users()](#M-CodeFirstWebFramework-AdminHelper-Users 'CodeFirstWebFramework.AdminHelper.Users')
  - [UsersListing()](#M-CodeFirstWebFramework-AdminHelper-UsersListing 'CodeFirstWebFramework.AdminHelper.UsersListing')
  - [permissions()](#M-CodeFirstWebFramework-AdminHelper-permissions-System-Int32- 'CodeFirstWebFramework.AdminHelper.permissions(System.Int32)')
- [AdminModule](#T-CodeFirstWebFramework-AdminModule 'CodeFirstWebFramework.AdminModule')
  - [Default()](#M-CodeFirstWebFramework-AdminModule-Default 'CodeFirstWebFramework.AdminModule.Default')
  - [Init()](#M-CodeFirstWebFramework-AdminModule-Init 'CodeFirstWebFramework.AdminModule.Init')
- [AjaxReturn](#T-CodeFirstWebFramework-AjaxReturn 'CodeFirstWebFramework.AjaxReturn')
  - [confirm](#F-CodeFirstWebFramework-AjaxReturn-confirm 'CodeFirstWebFramework.AjaxReturn.confirm')
  - [data](#F-CodeFirstWebFramework-AjaxReturn-data 'CodeFirstWebFramework.AjaxReturn.data')
  - [error](#F-CodeFirstWebFramework-AjaxReturn-error 'CodeFirstWebFramework.AjaxReturn.error')
  - [id](#F-CodeFirstWebFramework-AjaxReturn-id 'CodeFirstWebFramework.AjaxReturn.id')
  - [message](#F-CodeFirstWebFramework-AjaxReturn-message 'CodeFirstWebFramework.AjaxReturn.message')
  - [redirect](#F-CodeFirstWebFramework-AjaxReturn-redirect 'CodeFirstWebFramework.AjaxReturn.redirect')
  - [ToString()](#M-CodeFirstWebFramework-AjaxReturn-ToString 'CodeFirstWebFramework.AjaxReturn.ToString')
- [AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule')
  - [#ctor()](#M-CodeFirstWebFramework-AppModule-#ctor 'CodeFirstWebFramework.AppModule.#ctor')
  - [#ctor()](#M-CodeFirstWebFramework-AppModule-#ctor-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.AppModule.#ctor(CodeFirstWebFramework.AppModule)')
  - [ActiveModule](#F-CodeFirstWebFramework-AppModule-ActiveModule 'CodeFirstWebFramework.AppModule.ActiveModule')
  - [Batch](#F-CodeFirstWebFramework-AppModule-Batch 'CodeFirstWebFramework.AppModule.Batch')
  - [Body](#F-CodeFirstWebFramework-AppModule-Body 'CodeFirstWebFramework.AppModule.Body')
  - [Charset](#F-CodeFirstWebFramework-AppModule-Charset 'CodeFirstWebFramework.AppModule.Charset')
  - [Context](#F-CodeFirstWebFramework-AppModule-Context 'CodeFirstWebFramework.AppModule.Context')
  - [Encoding](#F-CodeFirstWebFramework-AppModule-Encoding 'CodeFirstWebFramework.AppModule.Encoding')
  - [Exception](#F-CodeFirstWebFramework-AppModule-Exception 'CodeFirstWebFramework.AppModule.Exception')
  - [Form](#F-CodeFirstWebFramework-AppModule-Form 'CodeFirstWebFramework.AppModule.Form')
  - [GetParameters](#F-CodeFirstWebFramework-AppModule-GetParameters 'CodeFirstWebFramework.AppModule.GetParameters')
  - [Head](#F-CodeFirstWebFramework-AppModule-Head 'CodeFirstWebFramework.AppModule.Head')
  - [HeaderScript](#F-CodeFirstWebFramework-AppModule-HeaderScript 'CodeFirstWebFramework.AppModule.HeaderScript')
  - [Info](#F-CodeFirstWebFramework-AppModule-Info 'CodeFirstWebFramework.AppModule.Info')
  - [LogString](#F-CodeFirstWebFramework-AppModule-LogString 'CodeFirstWebFramework.AppModule.LogString')
  - [Menu](#F-CodeFirstWebFramework-AppModule-Menu 'CodeFirstWebFramework.AppModule.Menu')
  - [Message](#F-CodeFirstWebFramework-AppModule-Message 'CodeFirstWebFramework.AppModule.Message')
  - [Method](#F-CodeFirstWebFramework-AppModule-Method 'CodeFirstWebFramework.AppModule.Method')
  - [Module](#F-CodeFirstWebFramework-AppModule-Module 'CodeFirstWebFramework.AppModule.Module')
  - [OriginalMethod](#F-CodeFirstWebFramework-AppModule-OriginalMethod 'CodeFirstWebFramework.AppModule.OriginalMethod')
  - [OriginalModule](#F-CodeFirstWebFramework-AppModule-OriginalModule 'CodeFirstWebFramework.AppModule.OriginalModule')
  - [Parameters](#F-CodeFirstWebFramework-AppModule-Parameters 'CodeFirstWebFramework.AppModule.Parameters')
  - [PostParameters](#F-CodeFirstWebFramework-AppModule-PostParameters 'CodeFirstWebFramework.AppModule.PostParameters')
  - [Server](#F-CodeFirstWebFramework-AppModule-Server 'CodeFirstWebFramework.AppModule.Server')
  - [Session](#F-CodeFirstWebFramework-AppModule-Session 'CodeFirstWebFramework.AppModule.Session')
  - [Title](#F-CodeFirstWebFramework-AppModule-Title 'CodeFirstWebFramework.AppModule.Title')
  - [UserAccessLevel](#F-CodeFirstWebFramework-AppModule-UserAccessLevel 'CodeFirstWebFramework.AppModule.UserAccessLevel')
  - [Admin](#P-CodeFirstWebFramework-AppModule-Admin 'CodeFirstWebFramework.AppModule.Admin')
  - [BatchJobItems](#P-CodeFirstWebFramework-AppModule-BatchJobItems 'CodeFirstWebFramework.AppModule.BatchJobItems')
  - [CacheAllowed](#P-CodeFirstWebFramework-AppModule-CacheAllowed 'CodeFirstWebFramework.AppModule.CacheAllowed')
  - [Config](#P-CodeFirstWebFramework-AppModule-Config 'CodeFirstWebFramework.AppModule.Config')
  - [CopyFrom](#P-CodeFirstWebFramework-AppModule-CopyFrom 'CodeFirstWebFramework.AppModule.CopyFrom')
  - [Database](#P-CodeFirstWebFramework-AppModule-Database 'CodeFirstWebFramework.AppModule.Database')
  - [Help](#P-CodeFirstWebFramework-AppModule-Help 'CodeFirstWebFramework.AppModule.Help')
  - [Jobs](#P-CodeFirstWebFramework-AppModule-Jobs 'CodeFirstWebFramework.AppModule.Jobs')
  - [Modules](#P-CodeFirstWebFramework-AppModule-Modules 'CodeFirstWebFramework.AppModule.Modules')
  - [ReadOnly](#P-CodeFirstWebFramework-AppModule-ReadOnly 'CodeFirstWebFramework.AppModule.ReadOnly')
  - [ReadWrite](#P-CodeFirstWebFramework-AppModule-ReadWrite 'CodeFirstWebFramework.AppModule.ReadWrite')
  - [Request](#P-CodeFirstWebFramework-AppModule-Request 'CodeFirstWebFramework.AppModule.Request')
  - [Response](#P-CodeFirstWebFramework-AppModule-Response 'CodeFirstWebFramework.AppModule.Response')
  - [ResponseSent](#P-CodeFirstWebFramework-AppModule-ResponseSent 'CodeFirstWebFramework.AppModule.ResponseSent')
  - [SecurityOn](#P-CodeFirstWebFramework-AppModule-SecurityOn 'CodeFirstWebFramework.AppModule.SecurityOn')
  - [SessionData](#P-CodeFirstWebFramework-AppModule-SessionData 'CodeFirstWebFramework.AppModule.SessionData')
  - [Settings](#P-CodeFirstWebFramework-AppModule-Settings 'CodeFirstWebFramework.AppModule.Settings')
  - [Today](#P-CodeFirstWebFramework-AppModule-Today 'CodeFirstWebFramework.AppModule.Today')
  - [VersionSuffix](#P-CodeFirstWebFramework-AppModule-VersionSuffix 'CodeFirstWebFramework.AppModule.VersionSuffix')
  - [Call()](#M-CodeFirstWebFramework-AppModule-Call-System-Net-HttpListenerContext,System-String,System-String- 'CodeFirstWebFramework.AppModule.Call(System.Net.HttpListenerContext,System.String,System.String)')
  - [CallMethod(method)](#M-CodeFirstWebFramework-AppModule-CallMethod-System-Reflection-MethodInfo@- 'CodeFirstWebFramework.AppModule.CallMethod(System.Reflection.MethodInfo@)')
  - [CloseDatabase()](#M-CodeFirstWebFramework-AppModule-CloseDatabase 'CodeFirstWebFramework.AppModule.CloseDatabase')
  - [ConvertEncoding(s)](#M-CodeFirstWebFramework-AppModule-ConvertEncoding-System-String- 'CodeFirstWebFramework.AppModule.ConvertEncoding(System.String)')
  - [Default()](#M-CodeFirstWebFramework-AppModule-Default 'CodeFirstWebFramework.AppModule.Default')
  - [DeleteRecord()](#M-CodeFirstWebFramework-AppModule-DeleteRecord-System-String,System-Int32- 'CodeFirstWebFramework.AppModule.DeleteRecord(System.String,System.Int32)')
  - [DirectoryInfo(foldername)](#M-CodeFirstWebFramework-AppModule-DirectoryInfo-System-String- 'CodeFirstWebFramework.AppModule.DirectoryInfo(System.String)')
  - [Dispose()](#M-CodeFirstWebFramework-AppModule-Dispose 'CodeFirstWebFramework.AppModule.Dispose')
  - [ExtractSection(name,template,defaultValue)](#M-CodeFirstWebFramework-AppModule-ExtractSection-System-String,System-String@,System-String- 'CodeFirstWebFramework.AppModule.ExtractSection(System.String,System.String@,System.String)')
  - [FileInfo(filename)](#M-CodeFirstWebFramework-AppModule-FileInfo-System-String- 'CodeFirstWebFramework.AppModule.FileInfo(System.String)')
  - [GetBatchJob()](#M-CodeFirstWebFramework-AppModule-GetBatchJob-System-Int32- 'CodeFirstWebFramework.AppModule.GetBatchJob(System.Int32)')
  - [HasAccess()](#M-CodeFirstWebFramework-AppModule-HasAccess-System-String- 'CodeFirstWebFramework.AppModule.HasAccess(System.String)')
  - [HasAccess(info,mtd,accessLevel)](#M-CodeFirstWebFramework-AppModule-HasAccess-CodeFirstWebFramework-ModuleInfo,System-String,System-Int32@- 'CodeFirstWebFramework.AppModule.HasAccess(CodeFirstWebFramework.ModuleInfo,System.String,System.Int32@)')
  - [Init()](#M-CodeFirstWebFramework-AppModule-Init 'CodeFirstWebFramework.AppModule.Init')
  - [InsertMenuOption()](#M-CodeFirstWebFramework-AppModule-InsertMenuOption-CodeFirstWebFramework-MenuOption- 'CodeFirstWebFramework.AppModule.InsertMenuOption(CodeFirstWebFramework.MenuOption)')
  - [InsertMenuOptions(opts)](#M-CodeFirstWebFramework-AppModule-InsertMenuOptions-CodeFirstWebFramework-MenuOption[]- 'CodeFirstWebFramework.AppModule.InsertMenuOptions(CodeFirstWebFramework.MenuOption[])')
  - [LoadFile(filename)](#M-CodeFirstWebFramework-AppModule-LoadFile-System-String- 'CodeFirstWebFramework.AppModule.LoadFile(System.String)')
  - [LoadFile()](#M-CodeFirstWebFramework-AppModule-LoadFile-CodeFirstWebFramework-IFileInfo- 'CodeFirstWebFramework.AppModule.LoadFile(CodeFirstWebFramework.IFileInfo)')
  - [LoadTemplate()](#M-CodeFirstWebFramework-AppModule-LoadTemplate-System-String,System-Object- 'CodeFirstWebFramework.AppModule.LoadTemplate(System.String,System.Object)')
  - [Log()](#M-CodeFirstWebFramework-AppModule-Log-System-String- 'CodeFirstWebFramework.AppModule.Log(System.String)')
  - [Log()](#M-CodeFirstWebFramework-AppModule-Log-System-String,System-Object[]- 'CodeFirstWebFramework.AppModule.Log(System.String,System.Object[])')
  - [Redirect()](#M-CodeFirstWebFramework-AppModule-Redirect-System-String- 'CodeFirstWebFramework.AppModule.Redirect(System.String)')
  - [ReloadSettings()](#M-CodeFirstWebFramework-AppModule-ReloadSettings 'CodeFirstWebFramework.AppModule.ReloadSettings')
  - [Respond()](#M-CodeFirstWebFramework-AppModule-Respond 'CodeFirstWebFramework.AppModule.Respond')
  - [SaveRecord()](#M-CodeFirstWebFramework-AppModule-SaveRecord-CodeFirstWebFramework-JsonObject- 'CodeFirstWebFramework.AppModule.SaveRecord(CodeFirstWebFramework.JsonObject)')
  - [Template()](#M-CodeFirstWebFramework-AppModule-Template-System-String,System-Object- 'CodeFirstWebFramework.AppModule.Template(System.String,System.Object)')
  - [TextTemplate(text,obj)](#M-CodeFirstWebFramework-AppModule-TextTemplate-System-String,System-Object- 'CodeFirstWebFramework.AppModule.TextTemplate(System.String,System.Object)')
  - [WriteResponse(o,contentType,status)](#M-CodeFirstWebFramework-AppModule-WriteResponse-System-Object,System-String,System-Net-HttpStatusCode- 'CodeFirstWebFramework.AppModule.WriteResponse(System.Object,System.String,System.Net.HttpStatusCode)')
- [AuthAttribute](#T-CodeFirstWebFramework-AuthAttribute 'CodeFirstWebFramework.AuthAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-AuthAttribute-#ctor 'CodeFirstWebFramework.AuthAttribute.#ctor')
  - [#ctor(AccessLevel)](#M-CodeFirstWebFramework-AuthAttribute-#ctor-System-Int32- 'CodeFirstWebFramework.AuthAttribute.#ctor(System.Int32)')
  - [AccessLevel](#F-CodeFirstWebFramework-AuthAttribute-AccessLevel 'CodeFirstWebFramework.AuthAttribute.AccessLevel')
  - [Hide](#F-CodeFirstWebFramework-AuthAttribute-Hide 'CodeFirstWebFramework.AuthAttribute.Hide')
  - [Name](#F-CodeFirstWebFramework-AuthAttribute-Name 'CodeFirstWebFramework.AuthAttribute.Name')
- [BaseForm](#T-CodeFirstWebFramework-BaseForm 'CodeFirstWebFramework.BaseForm')
  - [#ctor()](#M-CodeFirstWebFramework-BaseForm-#ctor-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.BaseForm.#ctor(CodeFirstWebFramework.AppModule)')
  - [Module](#F-CodeFirstWebFramework-BaseForm-Module 'CodeFirstWebFramework.BaseForm.Module')
  - [Options](#F-CodeFirstWebFramework-BaseForm-Options 'CodeFirstWebFramework.BaseForm.Options')
  - [Data](#P-CodeFirstWebFramework-BaseForm-Data 'CodeFirstWebFramework.BaseForm.Data')
  - [Show()](#M-CodeFirstWebFramework-BaseForm-Show 'CodeFirstWebFramework.BaseForm.Show')
  - [Show()](#M-CodeFirstWebFramework-BaseForm-Show-System-String- 'CodeFirstWebFramework.BaseForm.Show(System.String)')
- [BatchJob](#T-CodeFirstWebFramework-AppModule-BatchJob 'CodeFirstWebFramework.AppModule.BatchJob')
  - [#ctor(module,action)](#M-CodeFirstWebFramework-AppModule-BatchJob-#ctor-CodeFirstWebFramework-AppModule,System-Action- 'CodeFirstWebFramework.AppModule.BatchJob.#ctor(CodeFirstWebFramework.AppModule,System.Action)')
  - [#ctor(module,redirect,action)](#M-CodeFirstWebFramework-AppModule-BatchJob-#ctor-CodeFirstWebFramework-AppModule,System-String,System-Action- 'CodeFirstWebFramework.AppModule.BatchJob.#ctor(CodeFirstWebFramework.AppModule,System.String,System.Action)')
  - [Error](#F-CodeFirstWebFramework-AppModule-BatchJob-Error 'CodeFirstWebFramework.AppModule.BatchJob.Error')
  - [Finished](#F-CodeFirstWebFramework-AppModule-BatchJob-Finished 'CodeFirstWebFramework.AppModule.BatchJob.Finished')
  - [Records](#F-CodeFirstWebFramework-AppModule-BatchJob-Records 'CodeFirstWebFramework.AppModule.BatchJob.Records')
  - [Status](#F-CodeFirstWebFramework-AppModule-BatchJob-Status 'CodeFirstWebFramework.AppModule.BatchJob.Status')
  - [Id](#P-CodeFirstWebFramework-AppModule-BatchJob-Id 'CodeFirstWebFramework.AppModule.BatchJob.Id')
  - [PercentComplete](#P-CodeFirstWebFramework-AppModule-BatchJob-PercentComplete 'CodeFirstWebFramework.AppModule.BatchJob.PercentComplete')
  - [Record](#P-CodeFirstWebFramework-AppModule-BatchJob-Record 'CodeFirstWebFramework.AppModule.BatchJob.Record')
  - [Redirect](#P-CodeFirstWebFramework-AppModule-BatchJob-Redirect 'CodeFirstWebFramework.AppModule.BatchJob.Redirect')
- [BatchJobItem](#T-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem')
  - [Method](#F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Method 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem.Method')
  - [Module](#F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Module 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem.Module')
  - [Status](#F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Status 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem.Status')
  - [User](#F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-User 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem.User')
  - [idBatchJobItem](#F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-idBatchJobItem 'CodeFirstWebFramework.AppModule.BatchJob.BatchJobItem.idBatchJobItem')
- [CheckException](#T-CodeFirstWebFramework-CheckException 'CodeFirstWebFramework.CheckException')
  - [#ctor()](#M-CodeFirstWebFramework-CheckException-#ctor-System-String- 'CodeFirstWebFramework.CheckException.#ctor(System.String)')
  - [#ctor()](#M-CodeFirstWebFramework-CheckException-#ctor-System-String,System-Exception- 'CodeFirstWebFramework.CheckException.#ctor(System.String,System.Exception)')
  - [#ctor()](#M-CodeFirstWebFramework-CheckException-#ctor-System-Exception,System-String- 'CodeFirstWebFramework.CheckException.#ctor(System.Exception,System.String)')
  - [#ctor()](#M-CodeFirstWebFramework-CheckException-#ctor-System-String,System-Object[]- 'CodeFirstWebFramework.CheckException.#ctor(System.String,System.Object[])')
  - [#ctor()](#M-CodeFirstWebFramework-CheckException-#ctor-System-Exception,System-String,System-Object[]- 'CodeFirstWebFramework.CheckException.#ctor(System.Exception,System.String,System.Object[])')
- [Config](#T-CodeFirstWebFramework-Config 'CodeFirstWebFramework.Config')
- [Config](#T-CodeFirstWebFramework-Log-Config 'CodeFirstWebFramework.Log.Config')
  - [CommandLineFlags](#F-CodeFirstWebFramework-Config-CommandLineFlags 'CodeFirstWebFramework.Config.CommandLineFlags')
  - [ConnectionString](#F-CodeFirstWebFramework-Config-ConnectionString 'CodeFirstWebFramework.Config.ConnectionString')
  - [CookieTimeoutMinutes](#F-CodeFirstWebFramework-Config-CookieTimeoutMinutes 'CodeFirstWebFramework.Config.CookieTimeoutMinutes')
  - [DataPath](#F-CodeFirstWebFramework-Config-DataPath 'CodeFirstWebFramework.Config.DataPath')
  - [Database](#F-CodeFirstWebFramework-Config-Database 'CodeFirstWebFramework.Config.Database')
  - [Default](#F-CodeFirstWebFramework-Config-Default 'CodeFirstWebFramework.Config.Default')
  - [DefaultNamespace](#F-CodeFirstWebFramework-Config-DefaultNamespace 'CodeFirstWebFramework.Config.DefaultNamespace')
  - [Email](#F-CodeFirstWebFramework-Config-Email 'CodeFirstWebFramework.Config.Email')
  - [EntryModule](#F-CodeFirstWebFramework-Config-EntryModule 'CodeFirstWebFramework.Config.EntryModule')
  - [EntryNamespace](#F-CodeFirstWebFramework-Config-EntryNamespace 'CodeFirstWebFramework.Config.EntryNamespace')
  - [Filename](#F-CodeFirstWebFramework-Config-Filename 'CodeFirstWebFramework.Config.Filename')
  - [Logging](#F-CodeFirstWebFramework-Config-Logging 'CodeFirstWebFramework.Config.Logging')
  - [Namespace](#F-CodeFirstWebFramework-Config-Namespace 'CodeFirstWebFramework.Config.Namespace')
  - [Port](#F-CodeFirstWebFramework-Config-Port 'CodeFirstWebFramework.Config.Port')
  - [ServerName](#F-CodeFirstWebFramework-Config-ServerName 'CodeFirstWebFramework.Config.ServerName')
  - [Servers](#F-CodeFirstWebFramework-Config-Servers 'CodeFirstWebFramework.Config.Servers')
  - [SessionExpiryMinutes](#F-CodeFirstWebFramework-Config-SessionExpiryMinutes 'CodeFirstWebFramework.Config.SessionExpiryMinutes')
  - [SlowQuery](#F-CodeFirstWebFramework-Config-SlowQuery 'CodeFirstWebFramework.Config.SlowQuery')
  - [DefaultServer](#P-CodeFirstWebFramework-Config-DefaultServer 'CodeFirstWebFramework.Config.DefaultServer')
  - [Load(filename)](#M-CodeFirstWebFramework-Config-Load-System-String- 'CodeFirstWebFramework.Config.Load(System.String)')
  - [Load(args)](#M-CodeFirstWebFramework-Config-Load-System-String[]- 'CodeFirstWebFramework.Config.Load(System.String[])')
  - [Save(filename)](#M-CodeFirstWebFramework-Config-Save-System-String- 'CodeFirstWebFramework.Config.Save(System.String)')
  - [SettingsForHost()](#M-CodeFirstWebFramework-Config-SettingsForHost-System-Uri- 'CodeFirstWebFramework.Config.SettingsForHost(System.Uri)')
  - [Update()](#M-CodeFirstWebFramework-Log-Config-Update 'CodeFirstWebFramework.Log.Config.Update')
- [DataTableForm](#T-CodeFirstWebFramework-DataTableForm 'CodeFirstWebFramework.DataTableForm')
  - [#ctor(module,t)](#M-CodeFirstWebFramework-DataTableForm-#ctor-CodeFirstWebFramework-AppModule,System-Type- 'CodeFirstWebFramework.DataTableForm.#ctor(CodeFirstWebFramework.AppModule,System.Type)')
  - [#ctor()](#M-CodeFirstWebFramework-DataTableForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]- 'CodeFirstWebFramework.DataTableForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean,System.String[])')
  - [Select](#P-CodeFirstWebFramework-DataTableForm-Select 'CodeFirstWebFramework.DataTableForm.Select')
  - [RequireField()](#M-CodeFirstWebFramework-DataTableForm-RequireField-CodeFirstWebFramework-FieldAttribute- 'CodeFirstWebFramework.DataTableForm.RequireField(CodeFirstWebFramework.FieldAttribute)')
  - [Show()](#M-CodeFirstWebFramework-DataTableForm-Show 'CodeFirstWebFramework.DataTableForm.Show')
- [Database](#T-CodeFirstWebFramework-Database 'CodeFirstWebFramework.Database')
  - [#ctor(server)](#M-CodeFirstWebFramework-Database-#ctor-CodeFirstWebFramework-ServerConfig- 'CodeFirstWebFramework.Database.#ctor(CodeFirstWebFramework.ServerConfig)')
  - [Logging](#F-CodeFirstWebFramework-Database-Logging 'CodeFirstWebFramework.Database.Logging')
  - [Module](#F-CodeFirstWebFramework-Database-Module 'CodeFirstWebFramework.Database.Module')
  - [CurrentDbVersion](#P-CodeFirstWebFramework-Database-CurrentDbVersion 'CodeFirstWebFramework.Database.CurrentDbVersion')
  - [TableNames](#P-CodeFirstWebFramework-Database-TableNames 'CodeFirstWebFramework.Database.TableNames')
  - [UniqueIdentifier](#P-CodeFirstWebFramework-Database-UniqueIdentifier 'CodeFirstWebFramework.Database.UniqueIdentifier')
  - [ViewNames](#P-CodeFirstWebFramework-Database-ViewNames 'CodeFirstWebFramework.Database.ViewNames')
  - [BeginTransaction()](#M-CodeFirstWebFramework-Database-BeginTransaction 'CodeFirstWebFramework.Database.BeginTransaction')
  - [Cast()](#M-CodeFirstWebFramework-Database-Cast-System-String,System-String- 'CodeFirstWebFramework.Database.Cast(System.String,System.String)')
  - [CheckValidFieldname(f)](#M-CodeFirstWebFramework-Database-CheckValidFieldname-System-String- 'CodeFirstWebFramework.Database.CheckValidFieldname(System.String)')
  - [Clean()](#M-CodeFirstWebFramework-Database-Clean 'CodeFirstWebFramework.Database.Clean')
  - [Commit()](#M-CodeFirstWebFramework-Database-Commit 'CodeFirstWebFramework.Database.Commit')
  - [Delete(tableName,data)](#M-CodeFirstWebFramework-Database-Delete-System-String,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.Delete(System.String,Newtonsoft.Json.Linq.JObject)')
  - [Delete()](#M-CodeFirstWebFramework-Database-Delete-System-String,System-Int32- 'CodeFirstWebFramework.Database.Delete(System.String,System.Int32)')
  - [Delete(data)](#M-CodeFirstWebFramework-Database-Delete-CodeFirstWebFramework-JsonObject- 'CodeFirstWebFramework.Database.Delete(CodeFirstWebFramework.JsonObject)')
  - [Dispose()](#M-CodeFirstWebFramework-Database-Dispose 'CodeFirstWebFramework.Database.Dispose')
  - [EmptyRecord()](#M-CodeFirstWebFramework-Database-EmptyRecord-System-String- 'CodeFirstWebFramework.Database.EmptyRecord(System.String)')
  - [EmptyRecord\`\`1()](#M-CodeFirstWebFramework-Database-EmptyRecord``1 'CodeFirstWebFramework.Database.EmptyRecord``1')
  - [Execute()](#M-CodeFirstWebFramework-Database-Execute-System-String- 'CodeFirstWebFramework.Database.Execute(System.String)')
  - [Exists()](#M-CodeFirstWebFramework-Database-Exists-System-String,System-Nullable{System-Int32}- 'CodeFirstWebFramework.Database.Exists(System.String,System.Nullable{System.Int32})')
  - [ForeignKey()](#M-CodeFirstWebFramework-Database-ForeignKey-System-String,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.ForeignKey(System.String,Newtonsoft.Json.Linq.JObject)')
  - [ForeignKey()](#M-CodeFirstWebFramework-Database-ForeignKey-System-String,System-Object[]- 'CodeFirstWebFramework.Database.ForeignKey(System.String,System.Object[])')
  - [Get()](#M-CodeFirstWebFramework-Database-Get-System-String,System-Int32- 'CodeFirstWebFramework.Database.Get(System.String,System.Int32)')
  - [Get\`\`1()](#M-CodeFirstWebFramework-Database-Get``1-System-Int32- 'CodeFirstWebFramework.Database.Get``1(System.Int32)')
  - [Get\`\`1()](#M-CodeFirstWebFramework-Database-Get``1-``0- 'CodeFirstWebFramework.Database.Get``1(``0)')
  - [In()](#M-CodeFirstWebFramework-Database-In-System-Object[]- 'CodeFirstWebFramework.Database.In(System.Object[])')
  - [In\`\`1()](#M-CodeFirstWebFramework-Database-In``1-System-Collections-Generic-IEnumerable{``0}- 'CodeFirstWebFramework.Database.In``1(System.Collections.Generic.IEnumerable{``0})')
  - [Insert()](#M-CodeFirstWebFramework-Database-Insert-System-String,System-Collections-Generic-List{Newtonsoft-Json-Linq-JObject}- 'CodeFirstWebFramework.Database.Insert(System.String,System.Collections.Generic.List{Newtonsoft.Json.Linq.JObject})')
  - [Insert()](#M-CodeFirstWebFramework-Database-Insert-System-String,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.Insert(System.String,Newtonsoft.Json.Linq.JObject)')
  - [Insert()](#M-CodeFirstWebFramework-Database-Insert-System-String,CodeFirstWebFramework-JsonObject- 'CodeFirstWebFramework.Database.Insert(System.String,CodeFirstWebFramework.JsonObject)')
  - [Insert()](#M-CodeFirstWebFramework-Database-Insert-CodeFirstWebFramework-JsonObject- 'CodeFirstWebFramework.Database.Insert(CodeFirstWebFramework.JsonObject)')
  - [IsValidFieldname()](#M-CodeFirstWebFramework-Database-IsValidFieldname-System-String- 'CodeFirstWebFramework.Database.IsValidFieldname(System.String)')
  - [LookupKey()](#M-CodeFirstWebFramework-Database-LookupKey-System-String,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.LookupKey(System.String,Newtonsoft.Json.Linq.JObject)')
  - [LookupKey()](#M-CodeFirstWebFramework-Database-LookupKey-System-String,System-Object[]- 'CodeFirstWebFramework.Database.LookupKey(System.String,System.Object[])')
  - [PostUpgradeFromVersion(version)](#M-CodeFirstWebFramework-Database-PostUpgradeFromVersion-System-Int32- 'CodeFirstWebFramework.Database.PostUpgradeFromVersion(System.Int32)')
  - [PreUpgradeFromVersion(version)](#M-CodeFirstWebFramework-Database-PreUpgradeFromVersion-System-Int32- 'CodeFirstWebFramework.Database.PreUpgradeFromVersion(System.Int32)')
  - [Query()](#M-CodeFirstWebFramework-Database-Query-System-String- 'CodeFirstWebFramework.Database.Query(System.String)')
  - [Query(fields,conditions,tableNames)](#M-CodeFirstWebFramework-Database-Query-System-String,System-String,System-String[]- 'CodeFirstWebFramework.Database.Query(System.String,System.String,System.String[])')
  - [QueryOne()](#M-CodeFirstWebFramework-Database-QueryOne-System-String- 'CodeFirstWebFramework.Database.QueryOne(System.String)')
  - [QueryOne()](#M-CodeFirstWebFramework-Database-QueryOne-System-String,System-String,System-String[]- 'CodeFirstWebFramework.Database.QueryOne(System.String,System.String,System.String[])')
  - [QueryOne\`\`1()](#M-CodeFirstWebFramework-Database-QueryOne``1-System-String- 'CodeFirstWebFramework.Database.QueryOne``1(System.String)')
  - [QueryOne\`\`1()](#M-CodeFirstWebFramework-Database-QueryOne``1-System-String,System-String,System-String[]- 'CodeFirstWebFramework.Database.QueryOne``1(System.String,System.String,System.String[])')
  - [Query\`\`1()](#M-CodeFirstWebFramework-Database-Query``1-System-String- 'CodeFirstWebFramework.Database.Query``1(System.String)')
  - [Query\`\`1(fields,conditions,tableNames)](#M-CodeFirstWebFramework-Database-Query``1-System-String,System-String,System-String[]- 'CodeFirstWebFramework.Database.Query``1(System.String,System.String,System.String[])')
  - [Quote()](#M-CodeFirstWebFramework-Database-Quote-System-Object- 'CodeFirstWebFramework.Database.Quote(System.Object)')
  - [RecordExists()](#M-CodeFirstWebFramework-Database-RecordExists-System-String,System-Int32- 'CodeFirstWebFramework.Database.RecordExists(System.String,System.Int32)')
  - [RecordExists()](#M-CodeFirstWebFramework-Database-RecordExists-CodeFirstWebFramework-Table,System-Int32- 'CodeFirstWebFramework.Database.RecordExists(CodeFirstWebFramework.Table,System.Int32)')
  - [Rollback()](#M-CodeFirstWebFramework-Database-Rollback 'CodeFirstWebFramework.Database.Rollback')
  - [TableFor()](#M-CodeFirstWebFramework-Database-TableFor-System-String- 'CodeFirstWebFramework.Database.TableFor(System.String)')
  - [TableFor()](#M-CodeFirstWebFramework-Database-TableFor-System-Type- 'CodeFirstWebFramework.Database.TableFor(System.Type)')
  - [TableForOrDefault()](#M-CodeFirstWebFramework-Database-TableForOrDefault-System-Type- 'CodeFirstWebFramework.Database.TableForOrDefault(System.Type)')
  - [Update()](#M-CodeFirstWebFramework-Database-Update-System-String,System-Collections-Generic-List{Newtonsoft-Json-Linq-JObject}- 'CodeFirstWebFramework.Database.Update(System.String,System.Collections.Generic.List{Newtonsoft.Json.Linq.JObject})')
  - [Update()](#M-CodeFirstWebFramework-Database-Update-System-String,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.Update(System.String,Newtonsoft.Json.Linq.JObject)')
  - [Update()](#M-CodeFirstWebFramework-Database-Update-CodeFirstWebFramework-JsonObject- 'CodeFirstWebFramework.Database.Update(CodeFirstWebFramework.JsonObject)')
  - [Upgrade()](#M-CodeFirstWebFramework-Database-Upgrade 'CodeFirstWebFramework.Database.Upgrade')
  - [delete(table,data)](#M-CodeFirstWebFramework-Database-delete-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.delete(CodeFirstWebFramework.Table,Newtonsoft.Json.Linq.JObject)')
  - [update()](#M-CodeFirstWebFramework-Database-update-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.update(CodeFirstWebFramework.Table,Newtonsoft.Json.Linq.JObject)')
  - [updateIfChanged()](#M-CodeFirstWebFramework-Database-updateIfChanged-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Database.updateIfChanged(CodeFirstWebFramework.Table,Newtonsoft.Json.Linq.JObject)')
  - [upgrade(code,database)](#M-CodeFirstWebFramework-Database-upgrade-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table- 'CodeFirstWebFramework.Database.upgrade(CodeFirstWebFramework.Table,CodeFirstWebFramework.Table)')
- [DatabaseException](#T-CodeFirstWebFramework-DatabaseException 'CodeFirstWebFramework.DatabaseException')
  - [#ctor(ex,table)](#M-CodeFirstWebFramework-DatabaseException-#ctor-CodeFirstWebFramework-DatabaseException,CodeFirstWebFramework-Table- 'CodeFirstWebFramework.DatabaseException.#ctor(CodeFirstWebFramework.DatabaseException,CodeFirstWebFramework.Table)')
  - [#ctor(ex,sql)](#M-CodeFirstWebFramework-DatabaseException-#ctor-System-Exception,System-String- 'CodeFirstWebFramework.DatabaseException.#ctor(System.Exception,System.String)')
  - [Sql](#F-CodeFirstWebFramework-DatabaseException-Sql 'CodeFirstWebFramework.DatabaseException.Sql')
  - [Table](#F-CodeFirstWebFramework-DatabaseException-Table 'CodeFirstWebFramework.DatabaseException.Table')
  - [Message](#P-CodeFirstWebFramework-DatabaseException-Message 'CodeFirstWebFramework.DatabaseException.Message')
  - [ToString()](#M-CodeFirstWebFramework-DatabaseException-ToString 'CodeFirstWebFramework.DatabaseException.ToString')
- [DbInterface](#T-CodeFirstWebFramework-DbInterface 'CodeFirstWebFramework.DbInterface')
  - [Cast()](#M-CodeFirstWebFramework-DbInterface-Cast-System-String,System-String- 'CodeFirstWebFramework.DbInterface.Cast(System.String,System.String)')
  - [CleanDatabase()](#M-CodeFirstWebFramework-DbInterface-CleanDatabase 'CodeFirstWebFramework.DbInterface.CleanDatabase')
  - [Commit()](#M-CodeFirstWebFramework-DbInterface-Commit 'CodeFirstWebFramework.DbInterface.Commit')
  - [Execute()](#M-CodeFirstWebFramework-DbInterface-Execute-System-String- 'CodeFirstWebFramework.DbInterface.Execute(System.String)')
  - [FieldsMatch()](#M-CodeFirstWebFramework-DbInterface-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field- 'CodeFirstWebFramework.DbInterface.FieldsMatch(CodeFirstWebFramework.Table,CodeFirstWebFramework.Field,CodeFirstWebFramework.Field)')
  - [Insert()](#M-CodeFirstWebFramework-DbInterface-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean- 'CodeFirstWebFramework.DbInterface.Insert(CodeFirstWebFramework.Table,System.String,System.Boolean)')
  - [Quote()](#M-CodeFirstWebFramework-DbInterface-Quote-System-Object- 'CodeFirstWebFramework.DbInterface.Quote(System.Object)')
  - [Rollback()](#M-CodeFirstWebFramework-DbInterface-Rollback 'CodeFirstWebFramework.DbInterface.Rollback')
  - [Tables()](#M-CodeFirstWebFramework-DbInterface-Tables 'CodeFirstWebFramework.DbInterface.Tables')
  - [UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex)](#M-CodeFirstWebFramework-DbInterface-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}- 'CodeFirstWebFramework.DbInterface.UpgradeTable(CodeFirstWebFramework.Table,CodeFirstWebFramework.Table,System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Index},System.Collections.Generic.List{CodeFirstWebFramework.Index})')
  - [ViewsMatch()](#M-CodeFirstWebFramework-DbInterface-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View- 'CodeFirstWebFramework.DbInterface.ViewsMatch(CodeFirstWebFramework.View,CodeFirstWebFramework.View)')
- [DecimalFormatJsonConverter](#T-CodeFirstWebFramework-DecimalFormatJsonConverter 'CodeFirstWebFramework.DecimalFormatJsonConverter')
  - [#ctor()](#M-CodeFirstWebFramework-DecimalFormatJsonConverter-#ctor 'CodeFirstWebFramework.DecimalFormatJsonConverter.#ctor')
  - [CanConvert()](#M-CodeFirstWebFramework-DecimalFormatJsonConverter-CanConvert-System-Type- 'CodeFirstWebFramework.DecimalFormatJsonConverter.CanConvert(System.Type)')
  - [ReadJson()](#M-CodeFirstWebFramework-DecimalFormatJsonConverter-ReadJson-Newtonsoft-Json-JsonReader,System-Type,System-Object,Newtonsoft-Json-JsonSerializer- 'CodeFirstWebFramework.DecimalFormatJsonConverter.ReadJson(Newtonsoft.Json.JsonReader,System.Type,System.Object,Newtonsoft.Json.JsonSerializer)')
  - [WriteJson()](#M-CodeFirstWebFramework-DecimalFormatJsonConverter-WriteJson-Newtonsoft-Json-JsonWriter,System-Object,Newtonsoft-Json-JsonSerializer- 'CodeFirstWebFramework.DecimalFormatJsonConverter.WriteJson(Newtonsoft.Json.JsonWriter,System.Object,Newtonsoft.Json.JsonSerializer)')
- [DefaultValueAttribute](#T-CodeFirstWebFramework-DefaultValueAttribute 'CodeFirstWebFramework.DefaultValueAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-String- 'CodeFirstWebFramework.DefaultValueAttribute.#ctor(System.String)')
  - [#ctor()](#M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-Int32- 'CodeFirstWebFramework.DefaultValueAttribute.#ctor(System.Int32)')
  - [#ctor()](#M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-Boolean- 'CodeFirstWebFramework.DefaultValueAttribute.#ctor(System.Boolean)')
  - [Value](#F-CodeFirstWebFramework-DefaultValueAttribute-Value 'CodeFirstWebFramework.DefaultValueAttribute.Value')
- [Destination](#T-CodeFirstWebFramework-Log-Destination 'CodeFirstWebFramework.Log.Destination')
  - [Debug](#F-CodeFirstWebFramework-Log-Destination-Debug 'CodeFirstWebFramework.Log.Destination.Debug')
  - [File](#F-CodeFirstWebFramework-Log-Destination-File 'CodeFirstWebFramework.Log.Destination.File')
  - [Log](#F-CodeFirstWebFramework-Log-Destination-Log 'CodeFirstWebFramework.Log.Destination.Log')
  - [Null](#F-CodeFirstWebFramework-Log-Destination-Null 'CodeFirstWebFramework.Log.Destination.Null')
  - [StdErr](#F-CodeFirstWebFramework-Log-Destination-StdErr 'CodeFirstWebFramework.Log.Destination.StdErr')
  - [StdOut](#F-CodeFirstWebFramework-Log-Destination-StdOut 'CodeFirstWebFramework.Log.Destination.StdOut')
  - [Trace](#F-CodeFirstWebFramework-Log-Destination-Trace 'CodeFirstWebFramework.Log.Destination.Trace')
- [DirectoryInfo](#T-CodeFirstWebFramework-DirectoryInfo 'CodeFirstWebFramework.DirectoryInfo')
  - [#ctor()](#M-CodeFirstWebFramework-DirectoryInfo-#ctor-System-String,System-IO-DirectoryInfo- 'CodeFirstWebFramework.DirectoryInfo.#ctor(System.String,System.IO.DirectoryInfo)')
  - [Exists](#P-CodeFirstWebFramework-DirectoryInfo-Exists 'CodeFirstWebFramework.DirectoryInfo.Exists')
  - [Name](#P-CodeFirstWebFramework-DirectoryInfo-Name 'CodeFirstWebFramework.DirectoryInfo.Name')
  - [Path](#P-CodeFirstWebFramework-DirectoryInfo-Path 'CodeFirstWebFramework.DirectoryInfo.Path')
  - [Content()](#M-CodeFirstWebFramework-DirectoryInfo-Content-System-String- 'CodeFirstWebFramework.DirectoryInfo.Content(System.String)')
- [DoNotStoreAttribute](#T-CodeFirstWebFramework-DoNotStoreAttribute 'CodeFirstWebFramework.DoNotStoreAttribute')
- [DumbForm](#T-CodeFirstWebFramework-DumbForm 'CodeFirstWebFramework.DumbForm')
  - [#ctor(module,readwrite)](#M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Boolean- 'CodeFirstWebFramework.DumbForm.#ctor(CodeFirstWebFramework.AppModule,System.Boolean)')
  - [#ctor()](#M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type- 'CodeFirstWebFramework.DumbForm.#ctor(CodeFirstWebFramework.AppModule,System.Type)')
  - [#ctor()](#M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean- 'CodeFirstWebFramework.DumbForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean)')
  - [#ctor()](#M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]- 'CodeFirstWebFramework.DumbForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean,System.String[])')
  - [Show()](#M-CodeFirstWebFramework-DumbForm-Show 'CodeFirstWebFramework.DumbForm.Show')
- [ErrorModule](#T-CodeFirstWebFramework-ErrorModule 'CodeFirstWebFramework.ErrorModule')
- [Field](#T-CodeFirstWebFramework-Field 'CodeFirstWebFramework.Field')
  - [#ctor()](#M-CodeFirstWebFramework-Field-#ctor-System-String- 'CodeFirstWebFramework.Field.#ctor(System.String)')
  - [#ctor(name,type,length,nullable,autoIncrement,defaultValue)](#M-CodeFirstWebFramework-Field-#ctor-System-String,System-Type,System-Decimal,System-Boolean,System-Boolean,System-String- 'CodeFirstWebFramework.Field.#ctor(System.String,System.Type,System.Decimal,System.Boolean,System.Boolean,System.String)')
  - [ForeignKey](#F-CodeFirstWebFramework-Field-ForeignKey 'CodeFirstWebFramework.Field.ForeignKey')
  - [AutoIncrement](#P-CodeFirstWebFramework-Field-AutoIncrement 'CodeFirstWebFramework.Field.AutoIncrement')
  - [DefaultValue](#P-CodeFirstWebFramework-Field-DefaultValue 'CodeFirstWebFramework.Field.DefaultValue')
  - [Length](#P-CodeFirstWebFramework-Field-Length 'CodeFirstWebFramework.Field.Length')
  - [Name](#P-CodeFirstWebFramework-Field-Name 'CodeFirstWebFramework.Field.Name')
  - [Nullable](#P-CodeFirstWebFramework-Field-Nullable 'CodeFirstWebFramework.Field.Nullable')
  - [Type](#P-CodeFirstWebFramework-Field-Type 'CodeFirstWebFramework.Field.Type')
  - [TypeName](#P-CodeFirstWebFramework-Field-TypeName 'CodeFirstWebFramework.Field.TypeName')
  - [Data(view)](#M-CodeFirstWebFramework-Field-Data-System-Boolean- 'CodeFirstWebFramework.Field.Data(System.Boolean)')
  - [FieldFor(field,pk)](#M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-FieldInfo,CodeFirstWebFramework-PrimaryAttribute@- 'CodeFirstWebFramework.Field.FieldFor(System.Reflection.FieldInfo,CodeFirstWebFramework.PrimaryAttribute@)')
  - [FieldFor(field)](#M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-FieldInfo- 'CodeFirstWebFramework.Field.FieldFor(System.Reflection.FieldInfo)')
  - [FieldFor(field)](#M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-PropertyInfo- 'CodeFirstWebFramework.Field.FieldFor(System.Reflection.PropertyInfo)')
  - [FieldFor()](#M-CodeFirstWebFramework-Field-FieldFor-System-String,System-Type,System-Boolean,CodeFirstWebFramework-PrimaryAttribute,CodeFirstWebFramework-LengthAttribute,CodeFirstWebFramework-DefaultValueAttribute- 'CodeFirstWebFramework.Field.FieldFor(System.String,System.Type,System.Boolean,CodeFirstWebFramework.PrimaryAttribute,CodeFirstWebFramework.LengthAttribute,CodeFirstWebFramework.DefaultValueAttribute)')
  - [Quote()](#M-CodeFirstWebFramework-Field-Quote-System-Object- 'CodeFirstWebFramework.Field.Quote(System.Object)')
  - [ToString()](#M-CodeFirstWebFramework-Field-ToString 'CodeFirstWebFramework.Field.ToString')
- [FieldAttribute](#T-CodeFirstWebFramework-FieldAttribute 'CodeFirstWebFramework.FieldAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-FieldAttribute-#ctor 'CodeFirstWebFramework.FieldAttribute.#ctor')
  - [#ctor(args)](#M-CodeFirstWebFramework-FieldAttribute-#ctor-System-Object[]- 'CodeFirstWebFramework.FieldAttribute.#ctor(System.Object[])')
  - [Field](#F-CodeFirstWebFramework-FieldAttribute-Field 'CodeFirstWebFramework.FieldAttribute.Field')
  - [Options](#F-CodeFirstWebFramework-FieldAttribute-Options 'CodeFirstWebFramework.FieldAttribute.Options')
  - [Types](#F-CodeFirstWebFramework-FieldAttribute-Types 'CodeFirstWebFramework.FieldAttribute.Types')
  - [Attributes](#P-CodeFirstWebFramework-FieldAttribute-Attributes 'CodeFirstWebFramework.FieldAttribute.Attributes')
  - [Colspan](#P-CodeFirstWebFramework-FieldAttribute-Colspan 'CodeFirstWebFramework.FieldAttribute.Colspan')
  - [Data](#P-CodeFirstWebFramework-FieldAttribute-Data 'CodeFirstWebFramework.FieldAttribute.Data')
  - [FieldName](#P-CodeFirstWebFramework-FieldAttribute-FieldName 'CodeFirstWebFramework.FieldAttribute.FieldName')
  - [Heading](#P-CodeFirstWebFramework-FieldAttribute-Heading 'CodeFirstWebFramework.FieldAttribute.Heading')
  - [MaxLength](#P-CodeFirstWebFramework-FieldAttribute-MaxLength 'CodeFirstWebFramework.FieldAttribute.MaxLength')
  - [Name](#P-CodeFirstWebFramework-FieldAttribute-Name 'CodeFirstWebFramework.FieldAttribute.Name')
  - [SameRow](#P-CodeFirstWebFramework-FieldAttribute-SameRow 'CodeFirstWebFramework.FieldAttribute.SameRow')
  - [Type](#P-CodeFirstWebFramework-FieldAttribute-Type 'CodeFirstWebFramework.FieldAttribute.Type')
  - [Visible](#P-CodeFirstWebFramework-FieldAttribute-Visible 'CodeFirstWebFramework.FieldAttribute.Visible')
  - [FieldFor(db,field,readwrite)](#M-CodeFirstWebFramework-FieldAttribute-FieldFor-CodeFirstWebFramework-Database,System-Reflection-FieldInfo,System-Boolean- 'CodeFirstWebFramework.FieldAttribute.FieldFor(CodeFirstWebFramework.Database,System.Reflection.FieldInfo,System.Boolean)')
  - [FieldFor(field,readwrite)](#M-CodeFirstWebFramework-FieldAttribute-FieldFor-System-Reflection-PropertyInfo,System-Boolean- 'CodeFirstWebFramework.FieldAttribute.FieldFor(System.Reflection.PropertyInfo,System.Boolean)')
  - [MakeSelectable()](#M-CodeFirstWebFramework-FieldAttribute-MakeSelectable-CodeFirstWebFramework-JObjectEnumerable- 'CodeFirstWebFramework.FieldAttribute.MakeSelectable(CodeFirstWebFramework.JObjectEnumerable)')
  - [MakeSelectable()](#M-CodeFirstWebFramework-FieldAttribute-MakeSelectable-System-Collections-Generic-IEnumerable{Newtonsoft-Json-Linq-JObject}- 'CodeFirstWebFramework.FieldAttribute.MakeSelectable(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JObject})')
- [FileInfo](#T-CodeFirstWebFramework-FileInfo 'CodeFirstWebFramework.FileInfo')
  - [#ctor()](#M-CodeFirstWebFramework-FileInfo-#ctor-System-String,System-IO-FileInfo- 'CodeFirstWebFramework.FileInfo.#ctor(System.String,System.IO.FileInfo)')
  - [Exists](#P-CodeFirstWebFramework-FileInfo-Exists 'CodeFirstWebFramework.FileInfo.Exists')
  - [Extension](#P-CodeFirstWebFramework-FileInfo-Extension 'CodeFirstWebFramework.FileInfo.Extension')
  - [LastWriteTimeUtc](#P-CodeFirstWebFramework-FileInfo-LastWriteTimeUtc 'CodeFirstWebFramework.FileInfo.LastWriteTimeUtc')
  - [Name](#P-CodeFirstWebFramework-FileInfo-Name 'CodeFirstWebFramework.FileInfo.Name')
  - [Path](#P-CodeFirstWebFramework-FileInfo-Path 'CodeFirstWebFramework.FileInfo.Path')
  - [Content()](#M-CodeFirstWebFramework-FileInfo-Content-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.FileInfo.Content(CodeFirstWebFramework.AppModule)')
  - [Stream()](#M-CodeFirstWebFramework-FileInfo-Stream-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.FileInfo.Stream(CodeFirstWebFramework.AppModule)')
- [FileSender](#T-CodeFirstWebFramework-FileSender 'CodeFirstWebFramework.FileSender')
  - [#ctor()](#M-CodeFirstWebFramework-FileSender-#ctor-System-String- 'CodeFirstWebFramework.FileSender.#ctor(System.String)')
  - [ContentTypes](#F-CodeFirstWebFramework-FileSender-ContentTypes 'CodeFirstWebFramework.FileSender.ContentTypes')
  - [Filename](#F-CodeFirstWebFramework-FileSender-Filename 'CodeFirstWebFramework.FileSender.Filename')
  - [Default()](#M-CodeFirstWebFramework-FileSender-Default 'CodeFirstWebFramework.FileSender.Default')
- [FileSystem](#T-CodeFirstWebFramework-FileSystem 'CodeFirstWebFramework.FileSystem')
  - [DirectoryInfo(module,foldername)](#M-CodeFirstWebFramework-FileSystem-DirectoryInfo-CodeFirstWebFramework-AppModule,System-String- 'CodeFirstWebFramework.FileSystem.DirectoryInfo(CodeFirstWebFramework.AppModule,System.String)')
  - [FileInfo(module,filename)](#M-CodeFirstWebFramework-FileSystem-FileInfo-CodeFirstWebFramework-AppModule,System-String- 'CodeFirstWebFramework.FileSystem.FileInfo(CodeFirstWebFramework.AppModule,System.String)')
- [ForeignKey](#T-CodeFirstWebFramework-ForeignKey 'CodeFirstWebFramework.ForeignKey')
  - [#ctor(table,field)](#M-CodeFirstWebFramework-ForeignKey-#ctor-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field- 'CodeFirstWebFramework.ForeignKey.#ctor(CodeFirstWebFramework.Table,CodeFirstWebFramework.Field)')
  - [Field](#P-CodeFirstWebFramework-ForeignKey-Field 'CodeFirstWebFramework.ForeignKey.Field')
  - [Table](#P-CodeFirstWebFramework-ForeignKey-Table 'CodeFirstWebFramework.ForeignKey.Table')
- [ForeignKeyAttribute](#T-CodeFirstWebFramework-ForeignKeyAttribute 'CodeFirstWebFramework.ForeignKeyAttribute')
  - [#ctor(table)](#M-CodeFirstWebFramework-ForeignKeyAttribute-#ctor-System-String- 'CodeFirstWebFramework.ForeignKeyAttribute.#ctor(System.String)')
  - [Table](#P-CodeFirstWebFramework-ForeignKeyAttribute-Table 'CodeFirstWebFramework.ForeignKeyAttribute.Table')
- [Form](#T-CodeFirstWebFramework-Form 'CodeFirstWebFramework.Form')
  - [#ctor(module,readwrite)](#M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Boolean- 'CodeFirstWebFramework.Form.#ctor(CodeFirstWebFramework.AppModule,System.Boolean)')
  - [#ctor()](#M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type- 'CodeFirstWebFramework.Form.#ctor(CodeFirstWebFramework.AppModule,System.Type)')
  - [#ctor()](#M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean- 'CodeFirstWebFramework.Form.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean)')
  - [#ctor()](#M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]- 'CodeFirstWebFramework.Form.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean,System.String[])')
  - [ReadWrite](#F-CodeFirstWebFramework-Form-ReadWrite 'CodeFirstWebFramework.Form.ReadWrite')
  - [columns](#F-CodeFirstWebFramework-Form-columns 'CodeFirstWebFramework.Form.columns')
  - [CanDelete](#P-CodeFirstWebFramework-Form-CanDelete 'CodeFirstWebFramework.Form.CanDelete')
  - [Fields](#P-CodeFirstWebFramework-Form-Fields 'CodeFirstWebFramework.Form.Fields')
  - [Item](#P-CodeFirstWebFramework-Form-Item-System-String- 'CodeFirstWebFramework.Form.Item(System.String)')
  - [Add()](#M-CodeFirstWebFramework-Form-Add-System-Reflection-FieldInfo- 'CodeFirstWebFramework.Form.Add(System.Reflection.FieldInfo)')
  - [Add()](#M-CodeFirstWebFramework-Form-Add-System-Reflection-FieldInfo,System-Boolean- 'CodeFirstWebFramework.Form.Add(System.Reflection.FieldInfo,System.Boolean)')
  - [Add()](#M-CodeFirstWebFramework-Form-Add-CodeFirstWebFramework-FieldAttribute- 'CodeFirstWebFramework.Form.Add(CodeFirstWebFramework.FieldAttribute)')
  - [Add()](#M-CodeFirstWebFramework-Form-Add-System-Type,System-String- 'CodeFirstWebFramework.Form.Add(System.Type,System.String)')
  - [Build()](#M-CodeFirstWebFramework-Form-Build-System-Type- 'CodeFirstWebFramework.Form.Build(System.Type)')
  - [IndexOf()](#M-CodeFirstWebFramework-Form-IndexOf-System-String- 'CodeFirstWebFramework.Form.IndexOf(System.String)')
  - [Insert()](#M-CodeFirstWebFramework-Form-Insert-System-Int32,CodeFirstWebFramework-FieldAttribute- 'CodeFirstWebFramework.Form.Insert(System.Int32,CodeFirstWebFramework.FieldAttribute)')
  - [Remove()](#M-CodeFirstWebFramework-Form-Remove-System-String- 'CodeFirstWebFramework.Form.Remove(System.String)')
  - [Remove()](#M-CodeFirstWebFramework-Form-Remove-System-String[]- 'CodeFirstWebFramework.Form.Remove(System.String[])')
  - [Replace()](#M-CodeFirstWebFramework-Form-Replace-System-Int32,CodeFirstWebFramework-FieldAttribute- 'CodeFirstWebFramework.Form.Replace(System.Int32,CodeFirstWebFramework.FieldAttribute)')
  - [RequireField()](#M-CodeFirstWebFramework-Form-RequireField-CodeFirstWebFramework-FieldAttribute- 'CodeFirstWebFramework.Form.RequireField(CodeFirstWebFramework.FieldAttribute)')
  - [Show()](#M-CodeFirstWebFramework-Form-Show 'CodeFirstWebFramework.Form.Show')
  - [processFields(tbl,inTable)](#M-CodeFirstWebFramework-Form-processFields-System-Type,System-Boolean- 'CodeFirstWebFramework.Form.processFields(System.Type,System.Boolean)')
- [HandlesAttribute](#T-CodeFirstWebFramework-HandlesAttribute 'CodeFirstWebFramework.HandlesAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-HandlesAttribute-#ctor-System-String[]- 'CodeFirstWebFramework.HandlesAttribute.#ctor(System.String[])')
  - [Extensions](#F-CodeFirstWebFramework-HandlesAttribute-Extensions 'CodeFirstWebFramework.HandlesAttribute.Extensions')
- [HeaderDetailForm](#T-CodeFirstWebFramework-HeaderDetailForm 'CodeFirstWebFramework.HeaderDetailForm')
  - [#ctor(module,header,detail)](#M-CodeFirstWebFramework-HeaderDetailForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Type- 'CodeFirstWebFramework.HeaderDetailForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Type)')
  - [#ctor(module,header,detail)](#M-CodeFirstWebFramework-HeaderDetailForm-#ctor-CodeFirstWebFramework-AppModule,CodeFirstWebFramework-Form,CodeFirstWebFramework-ListForm- 'CodeFirstWebFramework.HeaderDetailForm.#ctor(CodeFirstWebFramework.AppModule,CodeFirstWebFramework.Form,CodeFirstWebFramework.ListForm)')
  - [Detail](#F-CodeFirstWebFramework-HeaderDetailForm-Detail 'CodeFirstWebFramework.HeaderDetailForm.Detail')
  - [Header](#F-CodeFirstWebFramework-HeaderDetailForm-Header 'CodeFirstWebFramework.HeaderDetailForm.Header')
  - [CanDelete](#P-CodeFirstWebFramework-HeaderDetailForm-CanDelete 'CodeFirstWebFramework.HeaderDetailForm.CanDelete')
  - [Show()](#M-CodeFirstWebFramework-HeaderDetailForm-Show 'CodeFirstWebFramework.HeaderDetailForm.Show')
- [Help](#T-CodeFirstWebFramework-Help 'CodeFirstWebFramework.Help')
  - [Contents](#F-CodeFirstWebFramework-Help-Contents 'CodeFirstWebFramework.Help.Contents')
  - [Next](#F-CodeFirstWebFramework-Help-Next 'CodeFirstWebFramework.Help.Next')
  - [Parent](#F-CodeFirstWebFramework-Help-Parent 'CodeFirstWebFramework.Help.Parent')
  - [Previous](#F-CodeFirstWebFramework-Help-Previous 'CodeFirstWebFramework.Help.Previous')
  - [CallMethod()](#M-CodeFirstWebFramework-Help-CallMethod-System-Reflection-MethodInfo@- 'CodeFirstWebFramework.Help.CallMethod(System.Reflection.MethodInfo@)')
  - [Default()](#M-CodeFirstWebFramework-Help-Default 'CodeFirstWebFramework.Help.Default')
  - [LoadHelpFrom()](#M-CodeFirstWebFramework-Help-LoadHelpFrom-CodeFirstWebFramework-IFileInfo- 'CodeFirstWebFramework.Help.LoadHelpFrom(CodeFirstWebFramework.IFileInfo)')
  - [ReturnHelpFrom()](#M-CodeFirstWebFramework-Help-ReturnHelpFrom-CodeFirstWebFramework-IFileInfo- 'CodeFirstWebFramework.Help.ReturnHelpFrom(CodeFirstWebFramework.IFileInfo)')
  - [parseContents(current)](#M-CodeFirstWebFramework-Help-parseContents-System-String- 'CodeFirstWebFramework.Help.parseContents(System.String)')
- [IDirectoryInfo](#T-CodeFirstWebFramework-IDirectoryInfo 'CodeFirstWebFramework.IDirectoryInfo')
  - [Exists](#P-CodeFirstWebFramework-IDirectoryInfo-Exists 'CodeFirstWebFramework.IDirectoryInfo.Exists')
  - [Name](#P-CodeFirstWebFramework-IDirectoryInfo-Name 'CodeFirstWebFramework.IDirectoryInfo.Name')
  - [Path](#P-CodeFirstWebFramework-IDirectoryInfo-Path 'CodeFirstWebFramework.IDirectoryInfo.Path')
  - [Content()](#M-CodeFirstWebFramework-IDirectoryInfo-Content-System-String- 'CodeFirstWebFramework.IDirectoryInfo.Content(System.String)')
- [IFileInfo](#T-CodeFirstWebFramework-IFileInfo 'CodeFirstWebFramework.IFileInfo')
  - [Exists](#P-CodeFirstWebFramework-IFileInfo-Exists 'CodeFirstWebFramework.IFileInfo.Exists')
  - [Extension](#P-CodeFirstWebFramework-IFileInfo-Extension 'CodeFirstWebFramework.IFileInfo.Extension')
  - [LastWriteTimeUtc](#P-CodeFirstWebFramework-IFileInfo-LastWriteTimeUtc 'CodeFirstWebFramework.IFileInfo.LastWriteTimeUtc')
  - [Name](#P-CodeFirstWebFramework-IFileInfo-Name 'CodeFirstWebFramework.IFileInfo.Name')
  - [Path](#P-CodeFirstWebFramework-IFileInfo-Path 'CodeFirstWebFramework.IFileInfo.Path')
  - [Content()](#M-CodeFirstWebFramework-IFileInfo-Content-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.IFileInfo.Content(CodeFirstWebFramework.AppModule)')
  - [Stream()](#M-CodeFirstWebFramework-IFileInfo-Stream-CodeFirstWebFramework-AppModule- 'CodeFirstWebFramework.IFileInfo.Stream(CodeFirstWebFramework.AppModule)')
- [ImplementationAttribute](#T-CodeFirstWebFramework-ImplementationAttribute 'CodeFirstWebFramework.ImplementationAttribute')
  - [#ctor(helperClass)](#M-CodeFirstWebFramework-ImplementationAttribute-#ctor-System-Type- 'CodeFirstWebFramework.ImplementationAttribute.#ctor(System.Type)')
  - [Helper](#P-CodeFirstWebFramework-ImplementationAttribute-Helper 'CodeFirstWebFramework.ImplementationAttribute.Helper')
- [Index](#T-CodeFirstWebFramework-Index 'CodeFirstWebFramework.Index')
  - [#ctor(name,fields)](#M-CodeFirstWebFramework-Index-#ctor-System-String,CodeFirstWebFramework-Field[]- 'CodeFirstWebFramework.Index.#ctor(System.String,CodeFirstWebFramework.Field[])')
  - [#ctor(name,fields)](#M-CodeFirstWebFramework-Index-#ctor-System-String,System-String[]- 'CodeFirstWebFramework.Index.#ctor(System.String,System.String[])')
  - [FieldList](#P-CodeFirstWebFramework-Index-FieldList 'CodeFirstWebFramework.Index.FieldList')
  - [Fields](#P-CodeFirstWebFramework-Index-Fields 'CodeFirstWebFramework.Index.Fields')
  - [Name](#P-CodeFirstWebFramework-Index-Name 'CodeFirstWebFramework.Index.Name')
  - [CoversData()](#M-CodeFirstWebFramework-Index-CoversData-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Index.CoversData(Newtonsoft.Json.Linq.JObject)')
  - [ToString()](#M-CodeFirstWebFramework-Index-ToString 'CodeFirstWebFramework.Index.ToString')
  - [Where()](#M-CodeFirstWebFramework-Index-Where-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Index.Where(Newtonsoft.Json.Linq.JObject)')
- [JObjectEnumerable](#T-CodeFirstWebFramework-JObjectEnumerable 'CodeFirstWebFramework.JObjectEnumerable')
  - [#ctor()](#M-CodeFirstWebFramework-JObjectEnumerable-#ctor-System-Collections-Generic-IEnumerable{Newtonsoft-Json-Linq-JObject}- 'CodeFirstWebFramework.JObjectEnumerable.#ctor(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JObject})')
  - [GetEnumerator()](#M-CodeFirstWebFramework-JObjectEnumerable-GetEnumerator 'CodeFirstWebFramework.JObjectEnumerable.GetEnumerator')
  - [System#Collections#IEnumerable#GetEnumerator()](#M-CodeFirstWebFramework-JObjectEnumerable-System#Collections#IEnumerable#GetEnumerator 'CodeFirstWebFramework.JObjectEnumerable.System#Collections#IEnumerable#GetEnumerator')
  - [ToList()](#M-CodeFirstWebFramework-JObjectEnumerable-ToList 'CodeFirstWebFramework.JObjectEnumerable.ToList')
  - [ToString()](#M-CodeFirstWebFramework-JObjectEnumerable-ToString 'CodeFirstWebFramework.JObjectEnumerable.ToString')
  - [op_Implicit()](#M-CodeFirstWebFramework-JObjectEnumerable-op_Implicit-CodeFirstWebFramework-JObjectEnumerable-~Newtonsoft-Json-Linq-JArray 'CodeFirstWebFramework.JObjectEnumerable.op_Implicit(CodeFirstWebFramework.JObjectEnumerable)~Newtonsoft.Json.Linq.JArray')
- [JsonObject](#T-CodeFirstWebFramework-JsonObject 'CodeFirstWebFramework.JsonObject')
  - [Id](#P-CodeFirstWebFramework-JsonObject-Id 'CodeFirstWebFramework.JsonObject.Id')
  - [Clone\`\`1()](#M-CodeFirstWebFramework-JsonObject-Clone``1 'CodeFirstWebFramework.JsonObject.Clone``1')
  - [ToJObject()](#M-CodeFirstWebFramework-JsonObject-ToJObject 'CodeFirstWebFramework.JsonObject.ToJObject')
  - [ToString()](#M-CodeFirstWebFramework-JsonObject-ToString 'CodeFirstWebFramework.JsonObject.ToString')
- [LengthAttribute](#T-CodeFirstWebFramework-LengthAttribute 'CodeFirstWebFramework.LengthAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-LengthAttribute-#ctor-System-Int32- 'CodeFirstWebFramework.LengthAttribute.#ctor(System.Int32)')
  - [#ctor()](#M-CodeFirstWebFramework-LengthAttribute-#ctor-System-Int32,System-Int32- 'CodeFirstWebFramework.LengthAttribute.#ctor(System.Int32,System.Int32)')
  - [Length](#F-CodeFirstWebFramework-LengthAttribute-Length 'CodeFirstWebFramework.LengthAttribute.Length')
  - [Precision](#F-CodeFirstWebFramework-LengthAttribute-Precision 'CodeFirstWebFramework.LengthAttribute.Precision')
- [ListForm](#T-CodeFirstWebFramework-ListForm 'CodeFirstWebFramework.ListForm')
  - [#ctor(module,t)](#M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type- 'CodeFirstWebFramework.ListForm.#ctor(CodeFirstWebFramework.AppModule,System.Type)')
  - [#ctor(module,t,readWrite)](#M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean- 'CodeFirstWebFramework.ListForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean)')
  - [#ctor()](#M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]- 'CodeFirstWebFramework.ListForm.#ctor(CodeFirstWebFramework.AppModule,System.Type,System.Boolean,System.String[])')
  - [Select](#P-CodeFirstWebFramework-ListForm-Select 'CodeFirstWebFramework.ListForm.Select')
  - [Show()](#M-CodeFirstWebFramework-ListForm-Show 'CodeFirstWebFramework.ListForm.Show')
- [Log](#T-CodeFirstWebFramework-Log 'CodeFirstWebFramework.Log')
  - [#ctor()](#M-CodeFirstWebFramework-Log-#ctor-CodeFirstWebFramework-Log-Destination- 'CodeFirstWebFramework.Log.#ctor(CodeFirstWebFramework.Log.Destination)')
  - [#ctor(config)](#M-CodeFirstWebFramework-Log-#ctor-System-String- 'CodeFirstWebFramework.Log.#ctor(System.String)')
  - [DatabaseRead](#F-CodeFirstWebFramework-Log-DatabaseRead 'CodeFirstWebFramework.Log.DatabaseRead')
  - [DatabaseWrite](#F-CodeFirstWebFramework-Log-DatabaseWrite 'CodeFirstWebFramework.Log.DatabaseWrite')
  - [Debug](#F-CodeFirstWebFramework-Log-Debug 'CodeFirstWebFramework.Log.Debug')
  - [Error](#F-CodeFirstWebFramework-Log-Error 'CodeFirstWebFramework.Log.Error')
  - [Info](#F-CodeFirstWebFramework-Log-Info 'CodeFirstWebFramework.Log.Info')
  - [LogFolder](#F-CodeFirstWebFramework-Log-LogFolder 'CodeFirstWebFramework.Log.LogFolder')
  - [NotFound](#F-CodeFirstWebFramework-Log-NotFound 'CodeFirstWebFramework.Log.NotFound')
  - [PostData](#F-CodeFirstWebFramework-Log-PostData 'CodeFirstWebFramework.Log.PostData')
  - [Session](#F-CodeFirstWebFramework-Log-Session 'CodeFirstWebFramework.Log.Session')
  - [Startup](#F-CodeFirstWebFramework-Log-Startup 'CodeFirstWebFramework.Log.Startup')
  - [Trace](#F-CodeFirstWebFramework-Log-Trace 'CodeFirstWebFramework.Log.Trace')
  - [On](#P-CodeFirstWebFramework-Log-On 'CodeFirstWebFramework.Log.On')
  - [Close()](#M-CodeFirstWebFramework-Log-Close 'CodeFirstWebFramework.Log.Close')
  - [Flush()](#M-CodeFirstWebFramework-Log-Flush 'CodeFirstWebFramework.Log.Flush')
  - [WriteLine()](#M-CodeFirstWebFramework-Log-WriteLine-System-String- 'CodeFirstWebFramework.Log.WriteLine(System.String)')
  - [WriteLine()](#M-CodeFirstWebFramework-Log-WriteLine-System-String,System-Object[]- 'CodeFirstWebFramework.Log.WriteLine(System.String,System.Object[])')
- [MenuOption](#T-CodeFirstWebFramework-MenuOption 'CodeFirstWebFramework.MenuOption')
  - [#ctor(text,url)](#M-CodeFirstWebFramework-MenuOption-#ctor-System-String,System-String- 'CodeFirstWebFramework.MenuOption.#ctor(System.String,System.String)')
  - [#ctor(text,url,enabled)](#M-CodeFirstWebFramework-MenuOption-#ctor-System-String,System-String,System-Boolean- 'CodeFirstWebFramework.MenuOption.#ctor(System.String,System.String,System.Boolean)')
  - [Enabled](#F-CodeFirstWebFramework-MenuOption-Enabled 'CodeFirstWebFramework.MenuOption.Enabled')
  - [Text](#F-CodeFirstWebFramework-MenuOption-Text 'CodeFirstWebFramework.MenuOption.Text')
  - [Url](#F-CodeFirstWebFramework-MenuOption-Url 'CodeFirstWebFramework.MenuOption.Url')
  - [Disabled](#P-CodeFirstWebFramework-MenuOption-Disabled 'CodeFirstWebFramework.MenuOption.Disabled')
  - [Id](#P-CodeFirstWebFramework-MenuOption-Id 'CodeFirstWebFramework.MenuOption.Id')
- [ModuleInfo](#T-CodeFirstWebFramework-ModuleInfo 'CodeFirstWebFramework.ModuleInfo')
  - [#ctor()](#M-CodeFirstWebFramework-ModuleInfo-#ctor-System-String,System-Type- 'CodeFirstWebFramework.ModuleInfo.#ctor(System.String,System.Type)')
  - [Auth](#F-CodeFirstWebFramework-ModuleInfo-Auth 'CodeFirstWebFramework.ModuleInfo.Auth')
  - [AuthMethods](#F-CodeFirstWebFramework-ModuleInfo-AuthMethods 'CodeFirstWebFramework.ModuleInfo.AuthMethods')
  - [ModuleAccessLevel](#F-CodeFirstWebFramework-ModuleInfo-ModuleAccessLevel 'CodeFirstWebFramework.ModuleInfo.ModuleAccessLevel')
  - [Name](#F-CodeFirstWebFramework-ModuleInfo-Name 'CodeFirstWebFramework.ModuleInfo.Name')
  - [Type](#F-CodeFirstWebFramework-ModuleInfo-Type 'CodeFirstWebFramework.ModuleInfo.Type')
  - [LowestAccessLevel](#P-CodeFirstWebFramework-ModuleInfo-LowestAccessLevel 'CodeFirstWebFramework.ModuleInfo.LowestAccessLevel')
  - [UnCamelName](#P-CodeFirstWebFramework-ModuleInfo-UnCamelName 'CodeFirstWebFramework.ModuleInfo.UnCamelName')
  - [addMethods(t)](#M-CodeFirstWebFramework-ModuleInfo-addMethods-System-Type- 'CodeFirstWebFramework.ModuleInfo.addMethods(System.Type)')
- [MySqlDatabase](#T-CodeFirstWebFramework-MySqlDatabase 'CodeFirstWebFramework.MySqlDatabase')
  - [#ctor(db,connectionString)](#M-CodeFirstWebFramework-MySqlDatabase-#ctor-CodeFirstWebFramework-Database,System-String- 'CodeFirstWebFramework.MySqlDatabase.#ctor(CodeFirstWebFramework.Database,System.String)')
  - [BeginTransaction()](#M-CodeFirstWebFramework-MySqlDatabase-BeginTransaction 'CodeFirstWebFramework.MySqlDatabase.BeginTransaction')
  - [Cast()](#M-CodeFirstWebFramework-MySqlDatabase-Cast-System-String,System-String- 'CodeFirstWebFramework.MySqlDatabase.Cast(System.String,System.String)')
  - [CleanDatabase()](#M-CodeFirstWebFramework-MySqlDatabase-CleanDatabase 'CodeFirstWebFramework.MySqlDatabase.CleanDatabase')
  - [Commit()](#M-CodeFirstWebFramework-MySqlDatabase-Commit 'CodeFirstWebFramework.MySqlDatabase.Commit')
  - [CreateIndex()](#M-CodeFirstWebFramework-MySqlDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.MySqlDatabase.CreateIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [CreateTable()](#M-CodeFirstWebFramework-MySqlDatabase-CreateTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.MySqlDatabase.CreateTable(CodeFirstWebFramework.Table)')
  - [Dispose()](#M-CodeFirstWebFramework-MySqlDatabase-Dispose 'CodeFirstWebFramework.MySqlDatabase.Dispose')
  - [DropIndex()](#M-CodeFirstWebFramework-MySqlDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.MySqlDatabase.DropIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [DropTable()](#M-CodeFirstWebFramework-MySqlDatabase-DropTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.MySqlDatabase.DropTable(CodeFirstWebFramework.Table)')
  - [Execute()](#M-CodeFirstWebFramework-MySqlDatabase-Execute-System-String- 'CodeFirstWebFramework.MySqlDatabase.Execute(System.String)')
  - [FieldsMatch()](#M-CodeFirstWebFramework-MySqlDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field- 'CodeFirstWebFramework.MySqlDatabase.FieldsMatch(CodeFirstWebFramework.Table,CodeFirstWebFramework.Field,CodeFirstWebFramework.Field)')
  - [Insert(table,sql,updatesAutoIncrement)](#M-CodeFirstWebFramework-MySqlDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean- 'CodeFirstWebFramework.MySqlDatabase.Insert(CodeFirstWebFramework.Table,System.String,System.Boolean)')
  - [Query()](#M-CodeFirstWebFramework-MySqlDatabase-Query-System-String- 'CodeFirstWebFramework.MySqlDatabase.Query(System.String)')
  - [QueryOne()](#M-CodeFirstWebFramework-MySqlDatabase-QueryOne-System-String- 'CodeFirstWebFramework.MySqlDatabase.QueryOne(System.String)')
  - [Quote()](#M-CodeFirstWebFramework-MySqlDatabase-Quote-System-Object- 'CodeFirstWebFramework.MySqlDatabase.Quote(System.Object)')
  - [Rollback()](#M-CodeFirstWebFramework-MySqlDatabase-Rollback 'CodeFirstWebFramework.MySqlDatabase.Rollback')
  - [Tables()](#M-CodeFirstWebFramework-MySqlDatabase-Tables 'CodeFirstWebFramework.MySqlDatabase.Tables')
  - [UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex)](#M-CodeFirstWebFramework-MySqlDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}- 'CodeFirstWebFramework.MySqlDatabase.UpgradeTable(CodeFirstWebFramework.Table,CodeFirstWebFramework.Table,System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Index},System.Collections.Generic.List{CodeFirstWebFramework.Index})')
  - [ViewsMatch()](#M-CodeFirstWebFramework-MySqlDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View- 'CodeFirstWebFramework.MySqlDatabase.ViewsMatch(CodeFirstWebFramework.View,CodeFirstWebFramework.View)')
- [Namespace](#T-CodeFirstWebFramework-Namespace 'CodeFirstWebFramework.Namespace')
  - [#ctor()](#M-CodeFirstWebFramework-Namespace-#ctor-CodeFirstWebFramework-ServerConfig- 'CodeFirstWebFramework.Namespace.#ctor(CodeFirstWebFramework.ServerConfig)')
  - [FileSystem](#F-CodeFirstWebFramework-Namespace-FileSystem 'CodeFirstWebFramework.Namespace.FileSystem')
  - [EmptySession](#P-CodeFirstWebFramework-Namespace-EmptySession 'CodeFirstWebFramework.Namespace.EmptySession')
  - [Modules](#P-CodeFirstWebFramework-Namespace-Modules 'CodeFirstWebFramework.Namespace.Modules')
  - [Name](#P-CodeFirstWebFramework-Namespace-Name 'CodeFirstWebFramework.Namespace.Name')
  - [TableNames](#P-CodeFirstWebFramework-Namespace-TableNames 'CodeFirstWebFramework.Namespace.TableNames')
  - [Tables](#P-CodeFirstWebFramework-Namespace-Tables 'CodeFirstWebFramework.Namespace.Tables')
  - [ViewNames](#P-CodeFirstWebFramework-Namespace-ViewNames 'CodeFirstWebFramework.Namespace.ViewNames')
  - [Create(server)](#M-CodeFirstWebFramework-Namespace-Create-CodeFirstWebFramework-ServerConfig- 'CodeFirstWebFramework.Namespace.Create(CodeFirstWebFramework.ServerConfig)')
  - [GetAccessLevel()](#M-CodeFirstWebFramework-Namespace-GetAccessLevel 'CodeFirstWebFramework.Namespace.GetAccessLevel')
  - [GetDatabase(server)](#M-CodeFirstWebFramework-Namespace-GetDatabase-CodeFirstWebFramework-ServerConfig- 'CodeFirstWebFramework.Namespace.GetDatabase(CodeFirstWebFramework.ServerConfig)')
  - [GetInstanceOf\`\`1(args)](#M-CodeFirstWebFramework-Namespace-GetInstanceOf``1-System-Object[]- 'CodeFirstWebFramework.Namespace.GetInstanceOf``1(System.Object[])')
  - [GetModuleInfo()](#M-CodeFirstWebFramework-Namespace-GetModuleInfo-System-String- 'CodeFirstWebFramework.Namespace.GetModuleInfo(System.String)')
  - [GetNamespaceType()](#M-CodeFirstWebFramework-Namespace-GetNamespaceType-System-Type- 'CodeFirstWebFramework.Namespace.GetNamespaceType(System.Type)')
  - [ParseUri()](#M-CodeFirstWebFramework-Namespace-ParseUri-System-String,System-String@- 'CodeFirstWebFramework.Namespace.ParseUri(System.String,System.String@)')
  - [TableFor()](#M-CodeFirstWebFramework-Namespace-TableFor-System-String- 'CodeFirstWebFramework.Namespace.TableFor(System.String)')
  - [processFields()](#M-CodeFirstWebFramework-Namespace-processFields-System-Type,System-Collections-Generic-List{CodeFirstWebFramework-Field}@,System-Collections-Generic-Dictionary{System-String,System-Collections-Generic-List{System-Tuple{System-Int32,CodeFirstWebFramework-Field}}}@,System-Collections-Generic-List{System-Tuple{System-Int32,CodeFirstWebFramework-Field}}@,System-String@- 'CodeFirstWebFramework.Namespace.processFields(System.Type,System.Collections.Generic.List{CodeFirstWebFramework.Field}@,System.Collections.Generic.Dictionary{System.String,System.Collections.Generic.List{System.Tuple{System.Int32,CodeFirstWebFramework.Field}}}@,System.Collections.Generic.List{System.Tuple{System.Int32,CodeFirstWebFramework.Field}}@,System.String@)')
  - [processTable()](#M-CodeFirstWebFramework-Namespace-processTable-System-Type,CodeFirstWebFramework-ViewAttribute- 'CodeFirstWebFramework.Namespace.processTable(System.Type,CodeFirstWebFramework.ViewAttribute)')
- [NullableAttribute](#T-CodeFirstWebFramework-NullableAttribute 'CodeFirstWebFramework.NullableAttribute')
- [Permission](#T-CodeFirstWebFramework-Permission 'CodeFirstWebFramework.Permission')
  - [FunctionAccessLevel](#F-CodeFirstWebFramework-Permission-FunctionAccessLevel 'CodeFirstWebFramework.Permission.FunctionAccessLevel')
  - [Method](#F-CodeFirstWebFramework-Permission-Method 'CodeFirstWebFramework.Permission.Method')
  - [MinAccessLevel](#F-CodeFirstWebFramework-Permission-MinAccessLevel 'CodeFirstWebFramework.Permission.MinAccessLevel')
  - [Module](#F-CodeFirstWebFramework-Permission-Module 'CodeFirstWebFramework.Permission.Module')
  - [UserId](#F-CodeFirstWebFramework-Permission-UserId 'CodeFirstWebFramework.Permission.UserId')
  - [Function](#P-CodeFirstWebFramework-Permission-Function 'CodeFirstWebFramework.Permission.Function')
- [PrimaryAttribute](#T-CodeFirstWebFramework-PrimaryAttribute 'CodeFirstWebFramework.PrimaryAttribute')
  - [#ctor()](#M-CodeFirstWebFramework-PrimaryAttribute-#ctor 'CodeFirstWebFramework.PrimaryAttribute.#ctor')
  - [#ctor(sequence)](#M-CodeFirstWebFramework-PrimaryAttribute-#ctor-System-Int32- 'CodeFirstWebFramework.PrimaryAttribute.#ctor(System.Int32)')
  - [AutoIncrement](#F-CodeFirstWebFramework-PrimaryAttribute-AutoIncrement 'CodeFirstWebFramework.PrimaryAttribute.AutoIncrement')
  - [Name](#P-CodeFirstWebFramework-PrimaryAttribute-Name 'CodeFirstWebFramework.PrimaryAttribute.Name')
  - [Sequence](#P-CodeFirstWebFramework-PrimaryAttribute-Sequence 'CodeFirstWebFramework.PrimaryAttribute.Sequence')
- [ReadOnlyAttribute](#T-CodeFirstWebFramework-ReadOnlyAttribute 'CodeFirstWebFramework.ReadOnlyAttribute')
- [SQLiteConcat](#T-CodeFirstWebFramework-SQLiteConcat 'CodeFirstWebFramework.SQLiteConcat')
- [SQLiteDatabase](#T-CodeFirstWebFramework-SQLiteDatabase 'CodeFirstWebFramework.SQLiteDatabase')
  - [#ctor()](#M-CodeFirstWebFramework-SQLiteDatabase-#ctor-CodeFirstWebFramework-Database,System-String- 'CodeFirstWebFramework.SQLiteDatabase.#ctor(CodeFirstWebFramework.Database,System.String)')
  - [#cctor()](#M-CodeFirstWebFramework-SQLiteDatabase-#cctor 'CodeFirstWebFramework.SQLiteDatabase.#cctor')
  - [BeginTransaction()](#M-CodeFirstWebFramework-SQLiteDatabase-BeginTransaction 'CodeFirstWebFramework.SQLiteDatabase.BeginTransaction')
  - [Cast()](#M-CodeFirstWebFramework-SQLiteDatabase-Cast-System-String,System-String- 'CodeFirstWebFramework.SQLiteDatabase.Cast(System.String,System.String)')
  - [CleanDatabase()](#M-CodeFirstWebFramework-SQLiteDatabase-CleanDatabase 'CodeFirstWebFramework.SQLiteDatabase.CleanDatabase')
  - [Commit()](#M-CodeFirstWebFramework-SQLiteDatabase-Commit 'CodeFirstWebFramework.SQLiteDatabase.Commit')
  - [CreateIndex()](#M-CodeFirstWebFramework-SQLiteDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.SQLiteDatabase.CreateIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [CreateTable()](#M-CodeFirstWebFramework-SQLiteDatabase-CreateTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.SQLiteDatabase.CreateTable(CodeFirstWebFramework.Table)')
  - [Dispose()](#M-CodeFirstWebFramework-SQLiteDatabase-Dispose 'CodeFirstWebFramework.SQLiteDatabase.Dispose')
  - [DropIndex()](#M-CodeFirstWebFramework-SQLiteDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.SQLiteDatabase.DropIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [DropTable()](#M-CodeFirstWebFramework-SQLiteDatabase-DropTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.SQLiteDatabase.DropTable(CodeFirstWebFramework.Table)')
  - [Execute()](#M-CodeFirstWebFramework-SQLiteDatabase-Execute-System-String- 'CodeFirstWebFramework.SQLiteDatabase.Execute(System.String)')
  - [FieldsMatch()](#M-CodeFirstWebFramework-SQLiteDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field- 'CodeFirstWebFramework.SQLiteDatabase.FieldsMatch(CodeFirstWebFramework.Table,CodeFirstWebFramework.Field,CodeFirstWebFramework.Field)')
  - [Insert(table,sql,updatesAutoIncrement)](#M-CodeFirstWebFramework-SQLiteDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean- 'CodeFirstWebFramework.SQLiteDatabase.Insert(CodeFirstWebFramework.Table,System.String,System.Boolean)')
  - [Query()](#M-CodeFirstWebFramework-SQLiteDatabase-Query-System-String- 'CodeFirstWebFramework.SQLiteDatabase.Query(System.String)')
  - [QueryOne()](#M-CodeFirstWebFramework-SQLiteDatabase-QueryOne-System-String- 'CodeFirstWebFramework.SQLiteDatabase.QueryOne(System.String)')
  - [Quote()](#M-CodeFirstWebFramework-SQLiteDatabase-Quote-System-Object- 'CodeFirstWebFramework.SQLiteDatabase.Quote(System.Object)')
  - [Rollback()](#M-CodeFirstWebFramework-SQLiteDatabase-Rollback 'CodeFirstWebFramework.SQLiteDatabase.Rollback')
  - [Tables()](#M-CodeFirstWebFramework-SQLiteDatabase-Tables 'CodeFirstWebFramework.SQLiteDatabase.Tables')
  - [UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex)](#M-CodeFirstWebFramework-SQLiteDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}- 'CodeFirstWebFramework.SQLiteDatabase.UpgradeTable(CodeFirstWebFramework.Table,CodeFirstWebFramework.Table,System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Index},System.Collections.Generic.List{CodeFirstWebFramework.Index})')
  - [ViewsMatch()](#M-CodeFirstWebFramework-SQLiteDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View- 'CodeFirstWebFramework.SQLiteDatabase.ViewsMatch(CodeFirstWebFramework.View,CodeFirstWebFramework.View)')
- [SQLiteDateDiff](#T-CodeFirstWebFramework-SQLiteDateDiff 'CodeFirstWebFramework.SQLiteDateDiff')
- [SQLiteSum](#T-CodeFirstWebFramework-SQLiteSum 'CodeFirstWebFramework.SQLiteSum')
- [ServerConfig](#T-CodeFirstWebFramework-ServerConfig 'CodeFirstWebFramework.ServerConfig')
  - [AdditionalAssemblies](#F-CodeFirstWebFramework-ServerConfig-AdditionalAssemblies 'CodeFirstWebFramework.ServerConfig.AdditionalAssemblies')
  - [ConnectionString](#F-CodeFirstWebFramework-ServerConfig-ConnectionString 'CodeFirstWebFramework.ServerConfig.ConnectionString')
  - [CookieTimeoutMinutes](#F-CodeFirstWebFramework-ServerConfig-CookieTimeoutMinutes 'CodeFirstWebFramework.ServerConfig.CookieTimeoutMinutes')
  - [Database](#F-CodeFirstWebFramework-ServerConfig-Database 'CodeFirstWebFramework.ServerConfig.Database')
  - [Email](#F-CodeFirstWebFramework-ServerConfig-Email 'CodeFirstWebFramework.ServerConfig.Email')
  - [Namespace](#F-CodeFirstWebFramework-ServerConfig-Namespace 'CodeFirstWebFramework.ServerConfig.Namespace')
  - [NamespaceDef](#F-CodeFirstWebFramework-ServerConfig-NamespaceDef 'CodeFirstWebFramework.ServerConfig.NamespaceDef')
  - [Port](#F-CodeFirstWebFramework-ServerConfig-Port 'CodeFirstWebFramework.ServerConfig.Port')
  - [ServerAlias](#F-CodeFirstWebFramework-ServerConfig-ServerAlias 'CodeFirstWebFramework.ServerConfig.ServerAlias')
  - [ServerName](#F-CodeFirstWebFramework-ServerConfig-ServerName 'CodeFirstWebFramework.ServerConfig.ServerName')
  - [Title](#F-CodeFirstWebFramework-ServerConfig-Title 'CodeFirstWebFramework.ServerConfig.Title')
  - [Matches()](#M-CodeFirstWebFramework-ServerConfig-Matches-System-Uri- 'CodeFirstWebFramework.ServerConfig.Matches(System.Uri)')
- [Session](#T-CodeFirstWebFramework-WebServer-Session 'CodeFirstWebFramework.WebServer.Session')
  - [#ctor(server)](#M-CodeFirstWebFramework-WebServer-Session-#ctor-CodeFirstWebFramework-WebServer- 'CodeFirstWebFramework.WebServer.Session.#ctor(CodeFirstWebFramework.WebServer)')
  - [Expires](#F-CodeFirstWebFramework-WebServer-Session-Expires 'CodeFirstWebFramework.WebServer.Session.Expires')
  - [Server](#F-CodeFirstWebFramework-WebServer-Session-Server 'CodeFirstWebFramework.WebServer.Session.Server')
  - [User](#F-CodeFirstWebFramework-WebServer-Session-User 'CodeFirstWebFramework.WebServer.Session.User')
  - [Cookie](#P-CodeFirstWebFramework-WebServer-Session-Cookie 'CodeFirstWebFramework.WebServer.Session.Cookie')
  - [Object](#P-CodeFirstWebFramework-WebServer-Session-Object 'CodeFirstWebFramework.WebServer.Session.Object')
  - [Dispose()](#M-CodeFirstWebFramework-WebServer-Session-Dispose 'CodeFirstWebFramework.WebServer.Session.Dispose')
- [Settings](#T-CodeFirstWebFramework-Settings 'CodeFirstWebFramework.Settings')
  - [DbVersion](#F-CodeFirstWebFramework-Settings-DbVersion 'CodeFirstWebFramework.Settings.DbVersion')
  - [Skin](#F-CodeFirstWebFramework-Settings-Skin 'CodeFirstWebFramework.Settings.Skin')
  - [idSettings](#F-CodeFirstWebFramework-Settings-idSettings 'CodeFirstWebFramework.Settings.idSettings')
  - [AppVersion](#P-CodeFirstWebFramework-Settings-AppVersion 'CodeFirstWebFramework.Settings.AppVersion')
  - [Id](#P-CodeFirstWebFramework-Settings-Id 'CodeFirstWebFramework.Settings.Id')
- [SqlServerDatabase](#T-CodeFirstWebFramework-SqlServerDatabase 'CodeFirstWebFramework.SqlServerDatabase')
  - [#ctor()](#M-CodeFirstWebFramework-SqlServerDatabase-#ctor-CodeFirstWebFramework-Database,System-String- 'CodeFirstWebFramework.SqlServerDatabase.#ctor(CodeFirstWebFramework.Database,System.String)')
  - [BeginTransaction()](#M-CodeFirstWebFramework-SqlServerDatabase-BeginTransaction 'CodeFirstWebFramework.SqlServerDatabase.BeginTransaction')
  - [Cast()](#M-CodeFirstWebFramework-SqlServerDatabase-Cast-System-String,System-String- 'CodeFirstWebFramework.SqlServerDatabase.Cast(System.String,System.String)')
  - [CleanDatabase()](#M-CodeFirstWebFramework-SqlServerDatabase-CleanDatabase 'CodeFirstWebFramework.SqlServerDatabase.CleanDatabase')
  - [Commit()](#M-CodeFirstWebFramework-SqlServerDatabase-Commit 'CodeFirstWebFramework.SqlServerDatabase.Commit')
  - [CreateIndex()](#M-CodeFirstWebFramework-SqlServerDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.SqlServerDatabase.CreateIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [CreateTable()](#M-CodeFirstWebFramework-SqlServerDatabase-CreateTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.SqlServerDatabase.CreateTable(CodeFirstWebFramework.Table)')
  - [Dispose()](#M-CodeFirstWebFramework-SqlServerDatabase-Dispose 'CodeFirstWebFramework.SqlServerDatabase.Dispose')
  - [DropIndex()](#M-CodeFirstWebFramework-SqlServerDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index- 'CodeFirstWebFramework.SqlServerDatabase.DropIndex(CodeFirstWebFramework.Table,CodeFirstWebFramework.Index)')
  - [DropTable()](#M-CodeFirstWebFramework-SqlServerDatabase-DropTable-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.SqlServerDatabase.DropTable(CodeFirstWebFramework.Table)')
  - [Execute()](#M-CodeFirstWebFramework-SqlServerDatabase-Execute-System-String- 'CodeFirstWebFramework.SqlServerDatabase.Execute(System.String)')
  - [FieldsMatch()](#M-CodeFirstWebFramework-SqlServerDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field- 'CodeFirstWebFramework.SqlServerDatabase.FieldsMatch(CodeFirstWebFramework.Table,CodeFirstWebFramework.Field,CodeFirstWebFramework.Field)')
  - [Insert(table,sql,updatesAutoIncrement)](#M-CodeFirstWebFramework-SqlServerDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean- 'CodeFirstWebFramework.SqlServerDatabase.Insert(CodeFirstWebFramework.Table,System.String,System.Boolean)')
  - [Query()](#M-CodeFirstWebFramework-SqlServerDatabase-Query-System-String- 'CodeFirstWebFramework.SqlServerDatabase.Query(System.String)')
  - [QueryOne()](#M-CodeFirstWebFramework-SqlServerDatabase-QueryOne-System-String- 'CodeFirstWebFramework.SqlServerDatabase.QueryOne(System.String)')
  - [Quote()](#M-CodeFirstWebFramework-SqlServerDatabase-Quote-System-Object- 'CodeFirstWebFramework.SqlServerDatabase.Quote(System.Object)')
  - [Rollback()](#M-CodeFirstWebFramework-SqlServerDatabase-Rollback 'CodeFirstWebFramework.SqlServerDatabase.Rollback')
  - [Tables()](#M-CodeFirstWebFramework-SqlServerDatabase-Tables 'CodeFirstWebFramework.SqlServerDatabase.Tables')
  - [UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex)](#M-CodeFirstWebFramework-SqlServerDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}- 'CodeFirstWebFramework.SqlServerDatabase.UpgradeTable(CodeFirstWebFramework.Table,CodeFirstWebFramework.Table,System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Field},System.Collections.Generic.List{CodeFirstWebFramework.Index},System.Collections.Generic.List{CodeFirstWebFramework.Index})')
  - [ViewsMatch()](#M-CodeFirstWebFramework-SqlServerDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View- 'CodeFirstWebFramework.SqlServerDatabase.ViewsMatch(CodeFirstWebFramework.View,CodeFirstWebFramework.View)')
- [Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table')
  - [#ctor()](#M-CodeFirstWebFramework-Table-#ctor-System-String,CodeFirstWebFramework-Field[],CodeFirstWebFramework-Index[]- 'CodeFirstWebFramework.Table.#ctor(System.String,CodeFirstWebFramework.Field[],CodeFirstWebFramework.Index[])')
  - [#ctor()](#M-CodeFirstWebFramework-Table-#ctor-System-Type- 'CodeFirstWebFramework.Table.#ctor(System.Type)')
  - [Fields](#F-CodeFirstWebFramework-Table-Fields 'CodeFirstWebFramework.Table.Fields')
  - [Type](#F-CodeFirstWebFramework-Table-Type 'CodeFirstWebFramework.Table.Type')
  - [Indexes](#P-CodeFirstWebFramework-Table-Indexes 'CodeFirstWebFramework.Table.Indexes')
  - [IsView](#P-CodeFirstWebFramework-Table-IsView 'CodeFirstWebFramework.Table.IsView')
  - [Name](#P-CodeFirstWebFramework-Table-Name 'CodeFirstWebFramework.Table.Name')
  - [PrimaryKey](#P-CodeFirstWebFramework-Table-PrimaryKey 'CodeFirstWebFramework.Table.PrimaryKey')
  - [UpdateTable](#P-CodeFirstWebFramework-Table-UpdateTable 'CodeFirstWebFramework.Table.UpdateTable')
  - [FieldFor()](#M-CodeFirstWebFramework-Table-FieldFor-System-String- 'CodeFirstWebFramework.Table.FieldFor(System.String)')
  - [ForeignKeyFieldFor()](#M-CodeFirstWebFramework-Table-ForeignKeyFieldFor-CodeFirstWebFramework-Table- 'CodeFirstWebFramework.Table.ForeignKeyFieldFor(CodeFirstWebFramework.Table)')
  - [FromJson\`\`1()](#M-CodeFirstWebFramework-Table-FromJson``1-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Table.FromJson``1(Newtonsoft.Json.Linq.JObject)')
  - [IndexFor()](#M-CodeFirstWebFramework-Table-IndexFor-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Table.IndexFor(Newtonsoft.Json.Linq.JObject)')
  - [ToString()](#M-CodeFirstWebFramework-Table-ToString 'CodeFirstWebFramework.Table.ToString')
- [TableAttribute](#T-CodeFirstWebFramework-TableAttribute 'CodeFirstWebFramework.TableAttribute')
- [TableList](#T-CodeFirstWebFramework-TableList 'CodeFirstWebFramework.TableList')
  - [#ctor(allTables)](#M-CodeFirstWebFramework-TableList-#ctor-System-Collections-Generic-IEnumerable{CodeFirstWebFramework-Table}- 'CodeFirstWebFramework.TableList.#ctor(System.Collections.Generic.IEnumerable{CodeFirstWebFramework.Table})')
- [TemplateSectionAttribute](#T-CodeFirstWebFramework-TemplateSectionAttribute 'CodeFirstWebFramework.TemplateSectionAttribute')
- [Timer](#T-CodeFirstWebFramework-Database-Timer 'CodeFirstWebFramework.Database.Timer')
  - [#ctor(message)](#M-CodeFirstWebFramework-Database-Timer-#ctor-System-String- 'CodeFirstWebFramework.Database.Timer.#ctor(System.String)')
  - [MaxTime](#F-CodeFirstWebFramework-Database-Timer-MaxTime 'CodeFirstWebFramework.Database.Timer.MaxTime')
  - [Dispose()](#M-CodeFirstWebFramework-Database-Timer-Dispose 'CodeFirstWebFramework.Database.Timer.Dispose')
- [UniqueAttribute](#T-CodeFirstWebFramework-UniqueAttribute 'CodeFirstWebFramework.UniqueAttribute')
  - [#ctor(name)](#M-CodeFirstWebFramework-UniqueAttribute-#ctor-System-String- 'CodeFirstWebFramework.UniqueAttribute.#ctor(System.String)')
  - [#ctor(name,sequence)](#M-CodeFirstWebFramework-UniqueAttribute-#ctor-System-String,System-Int32- 'CodeFirstWebFramework.UniqueAttribute.#ctor(System.String,System.Int32)')
  - [Name](#P-CodeFirstWebFramework-UniqueAttribute-Name 'CodeFirstWebFramework.UniqueAttribute.Name')
  - [Sequence](#P-CodeFirstWebFramework-UniqueAttribute-Sequence 'CodeFirstWebFramework.UniqueAttribute.Sequence')
- [UploadedFile](#T-CodeFirstWebFramework-UploadedFile 'CodeFirstWebFramework.UploadedFile')
  - [#ctor(name,content)](#M-CodeFirstWebFramework-UploadedFile-#ctor-System-String,System-String- 'CodeFirstWebFramework.UploadedFile.#ctor(System.String,System.String)')
  - [Content](#P-CodeFirstWebFramework-UploadedFile-Content 'CodeFirstWebFramework.UploadedFile.Content')
  - [Name](#P-CodeFirstWebFramework-UploadedFile-Name 'CodeFirstWebFramework.UploadedFile.Name')
  - [Stream()](#M-CodeFirstWebFramework-UploadedFile-Stream 'CodeFirstWebFramework.UploadedFile.Stream')
- [User](#T-CodeFirstWebFramework-User 'CodeFirstWebFramework.User')
  - [AccessLevel](#F-CodeFirstWebFramework-User-AccessLevel 'CodeFirstWebFramework.User.AccessLevel')
  - [Email](#F-CodeFirstWebFramework-User-Email 'CodeFirstWebFramework.User.Email')
  - [Login](#F-CodeFirstWebFramework-User-Login 'CodeFirstWebFramework.User.Login')
  - [ModulePermissions](#F-CodeFirstWebFramework-User-ModulePermissions 'CodeFirstWebFramework.User.ModulePermissions')
  - [Password](#F-CodeFirstWebFramework-User-Password 'CodeFirstWebFramework.User.Password')
  - [idUser](#F-CodeFirstWebFramework-User-idUser 'CodeFirstWebFramework.User.idUser')
  - [HashPassword()](#M-CodeFirstWebFramework-User-HashPassword-System-String- 'CodeFirstWebFramework.User.HashPassword(System.String)')
  - [PasswordValid()](#M-CodeFirstWebFramework-User-PasswordValid-System-String- 'CodeFirstWebFramework.User.PasswordValid(System.String)')
- [Utils](#T-CodeFirstWebFramework-Utils 'CodeFirstWebFramework.Utils')
  - [DecimalRegex](#F-CodeFirstWebFramework-Utils-DecimalRegex 'CodeFirstWebFramework.Utils.DecimalRegex')
  - [IntegerRegex](#F-CodeFirstWebFramework-Utils-IntegerRegex 'CodeFirstWebFramework.Utils.IntegerRegex')
  - [InvoiceNumber](#F-CodeFirstWebFramework-Utils-InvoiceNumber 'CodeFirstWebFramework.Utils.InvoiceNumber')
  - [_converter](#F-CodeFirstWebFramework-Utils-_converter 'CodeFirstWebFramework.Utils._converter')
  - [_timeOffset](#F-CodeFirstWebFramework-Utils-_timeOffset 'CodeFirstWebFramework.Utils._timeOffset')
  - [_tz](#F-CodeFirstWebFramework-Utils-_tz 'CodeFirstWebFramework.Utils._tz')
  - [Now](#P-CodeFirstWebFramework-Utils-Now 'CodeFirstWebFramework.Utils.Now')
  - [Today](#P-CodeFirstWebFramework-Utils-Today 'CodeFirstWebFramework.Utils.Today')
  - [AddRange()](#M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,System-Collections-Specialized-NameValueCollection- 'CodeFirstWebFramework.Utils.AddRange(Newtonsoft.Json.Linq.JObject,System.Collections.Specialized.NameValueCollection)')
  - [AddRange()](#M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Utils.AddRange(Newtonsoft.Json.Linq.JObject,Newtonsoft.Json.Linq.JObject)')
  - [AddRange(self,content)](#M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,System-Object[]- 'CodeFirstWebFramework.Utils.AddRange(Newtonsoft.Json.Linq.JObject,System.Object[])')
  - [AsBool()](#M-CodeFirstWebFramework-Utils-AsBool-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsBool(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsDate()](#M-CodeFirstWebFramework-Utils-AsDate-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsDate(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsDecimal()](#M-CodeFirstWebFramework-Utils-AsDecimal-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsDecimal(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsDouble()](#M-CodeFirstWebFramework-Utils-AsDouble-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsDouble(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsInt()](#M-CodeFirstWebFramework-Utils-AsInt-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsInt(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsJObject()](#M-CodeFirstWebFramework-Utils-AsJObject-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsJObject(Newtonsoft.Json.Linq.JObject,System.String)')
  - [AsString()](#M-CodeFirstWebFramework-Utils-AsString-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.AsString(Newtonsoft.Json.Linq.JObject,System.String)')
  - [As\`\`1()](#M-CodeFirstWebFramework-Utils-As``1-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.As``1(Newtonsoft.Json.Linq.JObject,System.String)')
  - [Capitalise()](#M-CodeFirstWebFramework-Utils-Capitalise-System-String- 'CodeFirstWebFramework.Utils.Capitalise(System.String)')
  - [Check()](#M-CodeFirstWebFramework-Utils-Check-System-Boolean,System-String- 'CodeFirstWebFramework.Utils.Check(System.Boolean,System.String)')
  - [Check()](#M-CodeFirstWebFramework-Utils-Check-System-Boolean,System-String,System-Object[]- 'CodeFirstWebFramework.Utils.Check(System.Boolean,System.String,System.Object[])')
  - [CopyFrom\`\`1()](#M-CodeFirstWebFramework-Utils-CopyFrom``1-``0,System-Object- 'CodeFirstWebFramework.Utils.CopyFrom``1(``0,System.Object)')
  - [ExtractNumber()](#M-CodeFirstWebFramework-Utils-ExtractNumber-System-String- 'CodeFirstWebFramework.Utils.ExtractNumber(System.String)')
  - [IsAllNull()](#M-CodeFirstWebFramework-Utils-IsAllNull-Newtonsoft-Json-Linq-JObject- 'CodeFirstWebFramework.Utils.IsAllNull(Newtonsoft.Json.Linq.JObject)')
  - [IsDecimal()](#M-CodeFirstWebFramework-Utils-IsDecimal-System-String- 'CodeFirstWebFramework.Utils.IsDecimal(System.String)')
  - [IsInteger()](#M-CodeFirstWebFramework-Utils-IsInteger-System-String- 'CodeFirstWebFramework.Utils.IsInteger(System.String)')
  - [IsMissingOrNull()](#M-CodeFirstWebFramework-Utils-IsMissingOrNull-Newtonsoft-Json-Linq-JObject,System-String- 'CodeFirstWebFramework.Utils.IsMissingOrNull(Newtonsoft.Json.Linq.JObject,System.String)')
  - [JsonTo()](#M-CodeFirstWebFramework-Utils-JsonTo-System-String,System-Type- 'CodeFirstWebFramework.Utils.JsonTo(System.String,System.Type)')
  - [JsonTo\`\`1()](#M-CodeFirstWebFramework-Utils-JsonTo``1-System-String- 'CodeFirstWebFramework.Utils.JsonTo``1(System.String)')
  - [Name()](#M-CodeFirstWebFramework-Utils-Name-System-Reflection-Assembly- 'CodeFirstWebFramework.Utils.Name(System.Reflection.Assembly)')
  - [NextToken()](#M-CodeFirstWebFramework-Utils-NextToken-System-String@,System-String[]- 'CodeFirstWebFramework.Utils.NextToken(System.String@,System.String[])')
  - [RemoveQuotes()](#M-CodeFirstWebFramework-Utils-RemoveQuotes-System-String- 'CodeFirstWebFramework.Utils.RemoveQuotes(System.String)')
  - [SimilarTo()](#M-CodeFirstWebFramework-Utils-SimilarTo-System-String,System-String- 'CodeFirstWebFramework.Utils.SimilarTo(System.String,System.String)')
  - [To()](#M-CodeFirstWebFramework-Utils-To-Newtonsoft-Json-Linq-JToken,System-Type- 'CodeFirstWebFramework.Utils.To(Newtonsoft.Json.Linq.JToken,System.Type)')
  - [ToJToken()](#M-CodeFirstWebFramework-Utils-ToJToken-System-Object- 'CodeFirstWebFramework.Utils.ToJToken(System.Object)')
  - [ToJson()](#M-CodeFirstWebFramework-Utils-ToJson-System-Object- 'CodeFirstWebFramework.Utils.ToJson(System.Object)')
  - [To\`\`1()](#M-CodeFirstWebFramework-Utils-To``1-Newtonsoft-Json-Linq-JToken- 'CodeFirstWebFramework.Utils.To``1(Newtonsoft.Json.Linq.JToken)')
  - [UnCamel()](#M-CodeFirstWebFramework-Utils-UnCamel-System-Object- 'CodeFirstWebFramework.Utils.UnCamel(System.Object)')
  - [UnCamel()](#M-CodeFirstWebFramework-Utils-UnCamel-System-String- 'CodeFirstWebFramework.Utils.UnCamel(System.String)')
- [View](#T-CodeFirstWebFramework-View 'CodeFirstWebFramework.View')
  - [#ctor(name,fields,indexes,sql,updateTable)](#M-CodeFirstWebFramework-View-#ctor-System-String,CodeFirstWebFramework-Field[],CodeFirstWebFramework-Index[],System-String,CodeFirstWebFramework-Table- 'CodeFirstWebFramework.View.#ctor(System.String,CodeFirstWebFramework.Field[],CodeFirstWebFramework.Index[],System.String,CodeFirstWebFramework.Table)')
  - [IsView](#P-CodeFirstWebFramework-View-IsView 'CodeFirstWebFramework.View.IsView')
  - [Sql](#P-CodeFirstWebFramework-View-Sql 'CodeFirstWebFramework.View.Sql')
  - [UpdateTable](#P-CodeFirstWebFramework-View-UpdateTable 'CodeFirstWebFramework.View.UpdateTable')
- [ViewAttribute](#T-CodeFirstWebFramework-ViewAttribute 'CodeFirstWebFramework.ViewAttribute')
  - [#ctor(sql)](#M-CodeFirstWebFramework-ViewAttribute-#ctor-System-String- 'CodeFirstWebFramework.ViewAttribute.#ctor(System.String)')
  - [Sql](#F-CodeFirstWebFramework-ViewAttribute-Sql 'CodeFirstWebFramework.ViewAttribute.Sql')
- [WebServer](#T-CodeFirstWebFramework-WebServer 'CodeFirstWebFramework.WebServer')
  - [#ctor()](#M-CodeFirstWebFramework-WebServer-#ctor 'CodeFirstWebFramework.WebServer.#ctor')
  - [AppVersion](#F-CodeFirstWebFramework-WebServer-AppVersion 'CodeFirstWebFramework.WebServer.AppVersion')
  - [VersionSuffix](#F-CodeFirstWebFramework-WebServer-VersionSuffix 'CodeFirstWebFramework.WebServer.VersionSuffix')
  - [Sessions](#P-CodeFirstWebFramework-WebServer-Sessions 'CodeFirstWebFramework.WebServer.Sessions')
  - [ProcessRequest(listenerContext)](#M-CodeFirstWebFramework-WebServer-ProcessRequest-System-Object- 'CodeFirstWebFramework.WebServer.ProcessRequest(System.Object)')
  - [Start()](#M-CodeFirstWebFramework-WebServer-Start 'CodeFirstWebFramework.WebServer.Start')
  - [Stop()](#M-CodeFirstWebFramework-WebServer-Stop 'CodeFirstWebFramework.WebServer.Stop')
  - [registerServer(databases,server)](#M-CodeFirstWebFramework-WebServer-registerServer-System-Collections-Generic-HashSet{System-String},CodeFirstWebFramework-ServerConfig- 'CodeFirstWebFramework.WebServer.registerServer(System.Collections.Generic.HashSet{System.String},CodeFirstWebFramework.ServerConfig)')
- [WriteableAttribute](#T-CodeFirstWebFramework-WriteableAttribute 'CodeFirstWebFramework.WriteableAttribute')

<a name='T-CodeFirstWebFramework-AccessLevel'></a>
## AccessLevel `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Predefined access levels.
Derive a class from this to provide additional levels.

<a name='F-CodeFirstWebFramework-AccessLevel-Admin'></a>
### Admin `constants`

##### Summary

Administrator - do not change this value - should allow access to anything

<a name='F-CodeFirstWebFramework-AccessLevel-Any'></a>
### Any `constants`

##### Summary

Allow access to anyone

<a name='F-CodeFirstWebFramework-AccessLevel-None'></a>
### None `constants`

##### Summary

// No access (a User.AccessLevel)

<a name='F-CodeFirstWebFramework-AccessLevel-ReadOnly'></a>
### ReadOnly `constants`

##### Summary

Read only

<a name='F-CodeFirstWebFramework-AccessLevel-ReadWrite'></a>
### ReadWrite `constants`

##### Summary

Read Write

<a name='F-CodeFirstWebFramework-AccessLevel-Unspecified'></a>
### Unspecified `constants`

##### Summary

No level specified - you will check the level in code, presumably

<a name='M-CodeFirstWebFramework-AccessLevel-Select'></a>
### Select() `method`

##### Summary

Return the options for a selectInput field to select AccessLevel.
NB Must always have 0, "None" as the first item.

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-AdminHelper'></a>
## AdminHelper `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class to provide Admin functions - called from Admin AppModule.

<a name='M-CodeFirstWebFramework-AdminHelper-#ctor-CodeFirstWebFramework-AppModule-'></a>
### #ctor() `constructor`

##### Summary

Create AdminHelper for supplied module

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Backup'></a>
### Backup() `method`

##### Summary

Backup the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Batch'></a>
### Batch() `method`

##### Summary

Display current or select batch details

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-BatchJobs'></a>
### BatchJobs() `method`

##### Summary

Display all running batch jobs

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-BatchStatus-System-Int32-'></a>
### BatchStatus() `method`

##### Summary

Return the status of the given batch job.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-ChangePassword'></a>
### ChangePassword() `method`

##### Summary

Create form to change user's password

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-ChangePasswordSave-Newtonsoft-Json-Linq-JObject-'></a>
### ChangePasswordSave() `method`

##### Summary

Update user's password

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-EditSettings'></a>
### EditSettings() `method`

##### Summary

Create form to edit settings

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-EditSettingsSave-Newtonsoft-Json-Linq-JObject-'></a>
### EditSettingsSave() `method`

##### Summary

Update settings

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-EditUser-System-Int32-'></a>
### EditUser() `method`

##### Summary

Create form to edit an individual user

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-EditUserDelete-System-Int32-'></a>
### EditUserDelete() `method`

##### Summary

Delete user

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-EditUserSave-Newtonsoft-Json-Linq-JObject-'></a>
### EditUserSave() `method`

##### Summary

Update user

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Login'></a>
### Login() `method`

##### Summary

Display login template, and log user in if form data is posted

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Logout'></a>
### Logout() `method`

##### Summary

Logout then show login form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Restore'></a>
### Restore() `method`

##### Summary

Restore the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-Users'></a>
### Users() `method`

##### Summary

Create datatable to list users

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-UsersListing'></a>
### UsersListing() `method`

##### Summary

List users for Users form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminHelper-permissions-System-Int32-'></a>
### permissions() `method`

##### Summary

List permissions for individual modules

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-AdminModule'></a>
## AdminModule `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Admin module - provides BatchStatus, Backup and Restore. Uses AdminHelper for the implementation.

<a name='M-CodeFirstWebFramework-AdminModule-Default'></a>
### Default() `method`

##### Summary

Display default template

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AdminModule-Init'></a>
### Init() `method`

##### Summary

Add menu options

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-AjaxReturn'></a>
## AjaxReturn `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Generic return type used for Ajax requests

<a name='F-CodeFirstWebFramework-AjaxReturn-confirm'></a>
### confirm `constants`

##### Summary

Ask the user to confirm something, and resubmit with confirm parameter if the user says yes

<a name='F-CodeFirstWebFramework-AjaxReturn-data'></a>
### data `constants`

##### Summary

Arbitrary data which the caller needs

<a name='F-CodeFirstWebFramework-AjaxReturn-error'></a>
### error `constants`

##### Summary

Exception message - if not null or empty, request has failed

<a name='F-CodeFirstWebFramework-AjaxReturn-id'></a>
### id `constants`

##### Summary

If a record has been saved, this is the id of the record.
Usually used to re-read the page, especially when the request was to create a new record.

<a name='F-CodeFirstWebFramework-AjaxReturn-message'></a>
### message `constants`

##### Summary

Message for user

<a name='F-CodeFirstWebFramework-AjaxReturn-redirect'></a>
### redirect `constants`

##### Summary

Where to redirect to on completion

<a name='M-CodeFirstWebFramework-AjaxReturn-ToString'></a>
### ToString() `method`

##### Summary

Show as a string (for logs)

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-AppModule'></a>
## AppModule `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Base class for all app modules.
Derive a class from this to server a folder of that name (you can add "Module" on the end of the name to avoid name clashes)
Create public methods to serve requests in that folder. If the method has arguments, the named arguments will be filled
in from the GET or POST request arguments (converting json to C# objects as required) - see Call below. 
If the method returns something, it will be returned using WriteResponse (below).
If the method is void, a template in the corresponding folder will be filled in, with the AppModule as the argument,
and returned.

<a name='M-CodeFirstWebFramework-AppModule-#ctor'></a>
### #ctor() `constructor`

##### Summary

Default constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-#ctor-CodeFirstWebFramework-AppModule-'></a>
### #ctor() `constructor`

##### Summary

Make a new module with settings copied from another

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-AppModule-ActiveModule'></a>
### ActiveModule `constants`

##### Summary

The Namespace in which this module is running

<a name='F-CodeFirstWebFramework-AppModule-Batch'></a>
### Batch `constants`

##### Summary

BatchJob started by this module

<a name='F-CodeFirstWebFramework-AppModule-Body'></a>
### Body `constants`

##### Summary

Goes into the web page body

<a name='F-CodeFirstWebFramework-AppModule-Charset'></a>
### Charset `constants`

##### Summary

Charset for web output

<a name='F-CodeFirstWebFramework-AppModule-Context'></a>
### Context `constants`

##### Summary

The Context from the web request that created this AppModule

<a name='F-CodeFirstWebFramework-AppModule-Encoding'></a>
### Encoding `constants`

##### Summary

Character encoding for web output

<a name='F-CodeFirstWebFramework-AppModule-Exception'></a>
### Exception `constants`

##### Summary

Any exception thrown handling a web request

<a name='F-CodeFirstWebFramework-AppModule-Form'></a>
### Form `constants`

##### Summary

The Form to render, if any

<a name='F-CodeFirstWebFramework-AppModule-GetParameters'></a>
### GetParameters `constants`

##### Summary

Parameters from Url

<a name='F-CodeFirstWebFramework-AppModule-Head'></a>
### Head `constants`

##### Summary

Goes into the web page header

<a name='F-CodeFirstWebFramework-AppModule-HeaderScript'></a>
### HeaderScript `constants`

##### Summary

Additional text to include in the template header.

<a name='F-CodeFirstWebFramework-AppModule-Info'></a>
### Info `constants`

##### Summary

Security information about this module

<a name='F-CodeFirstWebFramework-AppModule-LogString'></a>
### LogString `constants`

##### Summary

The data which is logged when the web request completes.

<a name='F-CodeFirstWebFramework-AppModule-Menu'></a>
### Menu `constants`

##### Summary

Module menu - line 2 of page top menu

<a name='F-CodeFirstWebFramework-AppModule-Message'></a>
### Message `constants`

##### Summary

Alert message to show user

<a name='F-CodeFirstWebFramework-AppModule-Method'></a>
### Method `constants`

##### Summary

The current Method (from the url - lower case).
Used by Respond to decide which template file to use.

<a name='F-CodeFirstWebFramework-AppModule-Module'></a>
### Module `constants`

##### Summary

The current module (from the url - lower case).
Used by Respond to decide which template file to use.

<a name='F-CodeFirstWebFramework-AppModule-OriginalMethod'></a>
### OriginalMethod `constants`

##### Summary

The original value of Method (from the url - lower case).

<a name='F-CodeFirstWebFramework-AppModule-OriginalModule'></a>
### OriginalModule `constants`

##### Summary

The original value of Module (from the url - lower case).

<a name='F-CodeFirstWebFramework-AppModule-Parameters'></a>
### Parameters `constants`

##### Summary

Get & Post parameters combined

<a name='F-CodeFirstWebFramework-AppModule-PostParameters'></a>
### PostParameters `constants`

##### Summary

Parameters from POST

<a name='F-CodeFirstWebFramework-AppModule-Server'></a>
### Server `constants`

##### Summary

The Server handling this request

<a name='F-CodeFirstWebFramework-AppModule-Session'></a>
### Session `constants`

##### Summary

So templates can access Session

<a name='F-CodeFirstWebFramework-AppModule-Title'></a>
### Title `constants`

##### Summary

Used for the web page title

<a name='F-CodeFirstWebFramework-AppModule-UserAccessLevel'></a>
### UserAccessLevel `constants`

##### Summary

Access level of the currently logged in user

<a name='P-CodeFirstWebFramework-AppModule-Admin'></a>
### Admin `property`

##### Summary

True if user has Admin access

<a name='P-CodeFirstWebFramework-AppModule-BatchJobItems'></a>
### BatchJobItems `property`

##### Summary

For displaying running batch jobs

<a name='P-CodeFirstWebFramework-AppModule-CacheAllowed'></a>
### CacheAllowed `property`

##### Summary

Set to true if the web server is allowed to cache this page. Normally false, as pages are generated dynamically.

<a name='P-CodeFirstWebFramework-AppModule-Config'></a>
### Config `property`

##### Summary

The Config file data

<a name='P-CodeFirstWebFramework-AppModule-CopyFrom'></a>
### CopyFrom `property`

##### Summary

Copy main settings from another AppModule

<a name='P-CodeFirstWebFramework-AppModule-Database'></a>
### Database `property`

##### Summary

The Database for this AppModule

<a name='P-CodeFirstWebFramework-AppModule-Help'></a>
### Help `property`

##### Summary

Return the url of any help file found in the help folder which applies to this module (and maybe method)

<a name='P-CodeFirstWebFramework-AppModule-Jobs'></a>
### Jobs `property`

##### Summary

All Modules running batch jobs

<a name='P-CodeFirstWebFramework-AppModule-Modules'></a>
### Modules `property`

##### Summary

List of modules for templates (e.g. to auto-generate a module menu)

<a name='P-CodeFirstWebFramework-AppModule-ReadOnly'></a>
### ReadOnly `property`

##### Summary

True if user does not have write access

<a name='P-CodeFirstWebFramework-AppModule-ReadWrite'></a>
### ReadWrite `property`

##### Summary

True if user does have write access

<a name='P-CodeFirstWebFramework-AppModule-Request'></a>
### Request `property`

##### Summary

The Web Request (from Context)

<a name='P-CodeFirstWebFramework-AppModule-Response'></a>
### Response `property`

##### Summary

The Web Response (from Context)

<a name='P-CodeFirstWebFramework-AppModule-ResponseSent'></a>
### ResponseSent `property`

##### Summary

True if a response has been sent to the request (and the default response should not be created)

<a name='P-CodeFirstWebFramework-AppModule-SecurityOn'></a>
### SecurityOn `property`

##### Summary

True if there are users in the database, so security should be checked

<a name='P-CodeFirstWebFramework-AppModule-SessionData'></a>
### SessionData `property`

##### Summary

Session data in dynamic form

<a name='P-CodeFirstWebFramework-AppModule-Settings'></a>
### Settings `property`

##### Summary

The Settings record from the database

<a name='P-CodeFirstWebFramework-AppModule-Today'></a>
### Today `property`

##### Summary

Today's date (yyyy-MM-dd)

<a name='P-CodeFirstWebFramework-AppModule-VersionSuffix'></a>
### VersionSuffix `property`

##### Summary

Version suffix for including in url's to defeat long-term caching of (e.g.) javascript and css files

<a name='M-CodeFirstWebFramework-AppModule-Call-System-Net-HttpListenerContext,System-String,System-String-'></a>
### Call() `method`

##### Summary

Responds to a Url request. Set up the AppModule variables and call the given method

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-CallMethod-System-Reflection-MethodInfo@-'></a>
### CallMethod(method) `method`

##### Summary

Call the method named by Method, and return its result

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| method | [System.Reflection.MethodInfo@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.MethodInfo@ 'System.Reflection.MethodInfo@') | Also return the MethodInfo so caller knows what return type it has.
Will be set to null if there is no such named method. |

<a name='M-CodeFirstWebFramework-AppModule-CloseDatabase'></a>
### CloseDatabase() `method`

##### Summary

Close the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-ConvertEncoding-System-String-'></a>
### ConvertEncoding(s) `method`

##### Summary

Convert a "binary" string (decoded using windows-1252) to the correct encoding

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| s | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-CodeFirstWebFramework-AppModule-Default'></a>
### Default() `method`

##### Summary

Method to call if no method supplied in url

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-DeleteRecord-System-String,System-Int32-'></a>
### DeleteRecord() `method`

##### Summary

Delete a record from the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-DirectoryInfo-System-String-'></a>
### DirectoryInfo(foldername) `method`

##### Summary

Get the IDirectoryInfo matching the foldername

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| foldername | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Like a url - e.g. "admin/settings" |

<a name='M-CodeFirstWebFramework-AppModule-Dispose'></a>
### Dispose() `method`

##### Summary

Close the database (unless a batch job is using it)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-ExtractSection-System-String,System-String@,System-String-'></a>
### ExtractSection(name,template,defaultValue) `method`

##### Summary

Extract the named html element from the template

##### Returns

The content of the element, or defaultValue

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Element to extract |
| template | [System.String@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String@ 'System.String@') | The template - will have the whole element removed |
| defaultValue | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Text to return if the named element is not present |

<a name='M-CodeFirstWebFramework-AppModule-FileInfo-System-String-'></a>
### FileInfo(filename) `method`

##### Summary

Get the IFileInfo matching the filename

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Like a url - e.g. "admin/settings.html" |

<a name='M-CodeFirstWebFramework-AppModule-GetBatchJob-System-Int32-'></a>
### GetBatchJob() `method`

##### Summary

Get batch job from id (for status/progress display)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-HasAccess-System-String-'></a>
### HasAccess() `method`

##### Summary

Check the security for access to a url

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-HasAccess-CodeFirstWebFramework-ModuleInfo,System-String,System-Int32@-'></a>
### HasAccess(info,mtd,accessLevel) `method`

##### Summary

Check the security for access to a method

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| info | [CodeFirstWebFramework.ModuleInfo](#T-CodeFirstWebFramework-ModuleInfo 'CodeFirstWebFramework.ModuleInfo') | The ModuleInfo for the relevant module |
| mtd | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The method name (lower case) |
| accessLevel | [System.Int32@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32@ 'System.Int32@') | The user's access level to this method |

<a name='M-CodeFirstWebFramework-AppModule-Init'></a>
### Init() `method`

##### Summary

Perform any initialisation or validation that applies to all calls to this module
(e.g. login or supervisor checks)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-InsertMenuOption-CodeFirstWebFramework-MenuOption-'></a>
### InsertMenuOption() `method`

##### Summary

Add a menu option to the default Menu (checking security - no add if no access)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-InsertMenuOptions-CodeFirstWebFramework-MenuOption[]-'></a>
### InsertMenuOptions(opts) `method`

##### Summary

Add multiple menu options to the default Menu (checking security - no add if no access)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| opts | [CodeFirstWebFramework.MenuOption[]](#T-CodeFirstWebFramework-MenuOption[] 'CodeFirstWebFramework.MenuOption[]') |  |

<a name='M-CodeFirstWebFramework-AppModule-LoadFile-System-String-'></a>
### LoadFile(filename) `method`

##### Summary

Load the contents of the file from one of the search folders.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Like a url |

<a name='M-CodeFirstWebFramework-AppModule-LoadFile-CodeFirstWebFramework-IFileInfo-'></a>
### LoadFile() `method`

##### Summary

Load the contents of a specific file.
If it is a .tmpl file, perform our extra substitutions to support {{include}}, //{{}}, '!{{}} and {{{}}}

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-LoadTemplate-System-String,System-Object-'></a>
### LoadTemplate() `method`

##### Summary

Load a template and perform Mustache substitutions on it using obj.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-Log-System-String-'></a>
### Log() `method`

##### Summary

Log to LogString (for showing in console with response data)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-Log-System-String,System-Object[]-'></a>
### Log() `method`

##### Summary

Log to LogString (for showing in console with response data)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-Redirect-System-String-'></a>
### Redirect() `method`

##### Summary

Perform a web redirect to redirect the browser to another url.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-ReloadSettings'></a>
### ReloadSettings() `method`

##### Summary

Force the Settings record to be reloaded from the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-Respond'></a>
### Respond() `method`

##### Summary

Render the template Module/Method.tmpl from this.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-SaveRecord-CodeFirstWebFramework-JsonObject-'></a>
### SaveRecord() `method`

##### Summary

Save an arbitrary JObject to the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-Template-System-String,System-Object-'></a>
### Template() `method`

##### Summary

Load the named template, and render using Mustache from the supplied object.
E.g. {{Body}} in the template will be replaced with the obj.Body.ToString()
Then split into <head> (goes to this.Head) and <body> (goes to this.Body)
(and also any other fields with the TemplateSection attribute).
If no body section, the whole remaining template goes into this.Body.
Then render the default template from this.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-AppModule-TextTemplate-System-String,System-Object-'></a>
### TextTemplate(text,obj) `method`

##### Summary

Perform Mustache substitutions on a template

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The template |
| obj | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The object to use |

<a name='M-CodeFirstWebFramework-AppModule-WriteResponse-System-Object,System-String,System-Net-HttpStatusCode-'></a>
### WriteResponse(o,contentType,status) `method`

##### Summary

Write the response to an Http request.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| o | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The object to write ("Operation complete" if null). 
May be a Stream, a string, a byte array or an object. If it is an object,
it is converted to json representation. |
| contentType | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The content type (suitable default is used if null) |
| status | [System.Net.HttpStatusCode](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Net.HttpStatusCode 'System.Net.HttpStatusCode') | The Http return code |

<a name='T-CodeFirstWebFramework-AuthAttribute'></a>
## AuthAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute to use on AppModule or method to limit access

<a name='M-CodeFirstWebFramework-AuthAttribute-#ctor'></a>
### #ctor() `constructor`

##### Summary

Constructor - level will be set to Any.

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-AuthAttribute-#ctor-System-Int32-'></a>
### #ctor(AccessLevel) `constructor`

##### Summary

Constructor with specific access level.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| AccessLevel | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') |  |

<a name='F-CodeFirstWebFramework-AuthAttribute-AccessLevel'></a>
### AccessLevel `constants`

##### Summary

The AccessLevel required to see this module or method.

<a name='F-CodeFirstWebFramework-AuthAttribute-Hide'></a>
### Hide `constants`

##### Summary

True if this AuthAttribute is not to appear on the list of module permissions

<a name='F-CodeFirstWebFramework-AuthAttribute-Name'></a>
### Name `constants`

##### Summary

Name to use (instead of module/method). AuthAttributes with the same name are grouped in the UI.

<a name='T-CodeFirstWebFramework-BaseForm'></a>
## BaseForm `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Base class for all supported forms

<a name='M-CodeFirstWebFramework-BaseForm-#ctor-CodeFirstWebFramework-AppModule-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-BaseForm-Module'></a>
### Module `constants`

##### Summary

Module creating the form

<a name='F-CodeFirstWebFramework-BaseForm-Options'></a>
### Options `constants`

##### Summary

Form options passed to javascript.

<a name='P-CodeFirstWebFramework-BaseForm-Data'></a>
### Data `property`

##### Summary

Form data passed to javascript.

<a name='M-CodeFirstWebFramework-BaseForm-Show'></a>
### Show() `method`

##### Summary

Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
exist, it uses the default template for the form (e.g. /datatable.tmpl).

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-BaseForm-Show-System-String-'></a>
### Show() `method`

##### Summary

Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
exist, it uses the default template for the form /formType.tmpl.

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-AppModule-BatchJob'></a>
## BatchJob `type`

##### Namespace

CodeFirstWebFramework.AppModule

##### Summary

Background batch job (e.g. import, restore)

<a name='M-CodeFirstWebFramework-AppModule-BatchJob-#ctor-CodeFirstWebFramework-AppModule,System-Action-'></a>
### #ctor(module,action) `constructor`

##### Summary

Create a batch job that redirects back to the module's original method on completion

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Module containing Database, Session, etc. |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') | Action to run the job |

<a name='M-CodeFirstWebFramework-AppModule-BatchJob-#ctor-CodeFirstWebFramework-AppModule,System-String,System-Action-'></a>
### #ctor(module,redirect,action) `constructor`

##### Summary

Create a batch job that redirects somewhere specific

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Module containing Database, Session, etc. |
| redirect | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Where to redirect to after batch (or null for default) |
| action | [System.Action](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Action 'System.Action') | Action to run the job |

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-Error'></a>
### Error `constants`

##### Summary

Error message (e.g. on exception)

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-Finished'></a>
### Finished `constants`

##### Summary

True if the batch job has finished

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-Records'></a>
### Records `constants`

##### Summary

Total number of records (for progress bar)

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-Status'></a>
### Status `constants`

##### Summary

For status/progress display

<a name='P-CodeFirstWebFramework-AppModule-BatchJob-Id'></a>
### Id `property`

##### Summary

Job id

<a name='P-CodeFirstWebFramework-AppModule-BatchJob-PercentComplete'></a>
### PercentComplete `property`

##### Summary

For progress display

<a name='P-CodeFirstWebFramework-AppModule-BatchJob-Record'></a>
### Record `property`

##### Summary

To indicate progress (0...Records)

<a name='P-CodeFirstWebFramework-AppModule-BatchJob-Redirect'></a>
### Redirect `property`

##### Summary

Where redirecting to on completion

<a name='T-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem'></a>
## BatchJobItem `type`

##### Namespace

CodeFirstWebFramework.AppModule.BatchJob

##### Summary

Class to list all running batch jobs

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Method'></a>
### Method `constants`

##### Summary

The method name

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Module'></a>
### Module `constants`

##### Summary

The module name

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-Status'></a>
### Status `constants`

##### Summary

The job status

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-User'></a>
### User `constants`

##### Summary

The user login

<a name='F-CodeFirstWebFramework-AppModule-BatchJob-BatchJobItem-idBatchJobItem'></a>
### idBatchJobItem `constants`

##### Summary

The id

<a name='T-CodeFirstWebFramework-CheckException'></a>
## CheckException `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Exception thrown by Check assertion function

<a name='M-CodeFirstWebFramework-CheckException-#ctor-System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-CheckException-#ctor-System-String,System-Exception-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-CheckException-#ctor-System-Exception,System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-CheckException-#ctor-System-String,System-Object[]-'></a>
### #ctor() `constructor`

##### Summary

Constructor accepting string.Format arguments

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-CheckException-#ctor-System-Exception,System-String,System-Object[]-'></a>
### #ctor() `constructor`

##### Summary

Constructor accepting string.Format arguments

##### Parameters

This constructor has no parameters.

<a name='T-CodeFirstWebFramework-Config'></a>
## Config `type`

##### Namespace

CodeFirstWebFramework

##### Summary

The config file from the data folder

<a name='T-CodeFirstWebFramework-Log-Config'></a>
## Config `type`

##### Namespace

CodeFirstWebFramework.Log

##### Summary

Strings to describe log destinations

<a name='F-CodeFirstWebFramework-Config-CommandLineFlags'></a>
### CommandLineFlags `constants`

##### Summary

Command line flags extracted from program command line

<a name='F-CodeFirstWebFramework-Config-ConnectionString'></a>
### ConnectionString `constants`

##### Summary

The default connection string

<a name='F-CodeFirstWebFramework-Config-CookieTimeoutMinutes'></a>
### CookieTimeoutMinutes `constants`

##### Summary

Cookie timeout in minutes

<a name='F-CodeFirstWebFramework-Config-DataPath'></a>
### DataPath `constants`

##### Summary

The data folder

<a name='F-CodeFirstWebFramework-Config-Database'></a>
### Database `constants`

##### Summary

The default type of database

<a name='F-CodeFirstWebFramework-Config-Default'></a>
### Default `constants`

##### Summary

The default (and only) Config file

<a name='F-CodeFirstWebFramework-Config-DefaultNamespace'></a>
### DefaultNamespace `constants`

##### Summary

The default namespace to use if none supplied

<a name='F-CodeFirstWebFramework-Config-Email'></a>
### Email `constants`

##### Summary

The default email address to send from

<a name='F-CodeFirstWebFramework-Config-EntryModule'></a>
### EntryModule `constants`

##### Summary

The name of the program

<a name='F-CodeFirstWebFramework-Config-EntryNamespace'></a>
### EntryNamespace `constants`

##### Summary

The namespace of the entry program

<a name='F-CodeFirstWebFramework-Config-Filename'></a>
### Filename `constants`

##### Summary

The name of the file from which this config has been read

<a name='F-CodeFirstWebFramework-Config-Logging'></a>
### Logging `constants`

##### Summary

Logging configuration

<a name='F-CodeFirstWebFramework-Config-Namespace'></a>
### Namespace `constants`

##### Summary

The default namespace

<a name='F-CodeFirstWebFramework-Config-Port'></a>
### Port `constants`

##### Summary

The default port the web server listens on

<a name='F-CodeFirstWebFramework-Config-ServerName'></a>
### ServerName `constants`

##### Summary

The default server name

<a name='F-CodeFirstWebFramework-Config-Servers'></a>
### Servers `constants`

##### Summary

List of other servers listening

<a name='F-CodeFirstWebFramework-Config-SessionExpiryMinutes'></a>
### SessionExpiryMinutes `constants`

##### Summary

Expire sessions after this number of minutes

<a name='F-CodeFirstWebFramework-Config-SlowQuery'></a>
### SlowQuery `constants`

##### Summary

Log all queries that take longer than this

<a name='P-CodeFirstWebFramework-Config-DefaultServer'></a>
### DefaultServer `property`

##### Summary

A ServerConfig with the defaults from the main Config section

<a name='M-CodeFirstWebFramework-Config-Load-System-String-'></a>
### Load(filename) `method`

##### Summary

Load a config by name

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Plain filename - no folders allowed - will be in the data folder |

<a name='M-CodeFirstWebFramework-Config-Load-System-String[]-'></a>
### Load(args) `method`

##### Summary

Read any config file specified in the command line, or ProgramName.config if none.
Also fill in the CommandLineFlags.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [System.String[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String[] 'System.String[]') | The program arguments |

<a name='M-CodeFirstWebFramework-Config-Save-System-String-'></a>
### Save(filename) `method`

##### Summary

Save this configuration by name

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Simple name - no folders allowed - it will be saved in the data folder |

<a name='M-CodeFirstWebFramework-Config-SettingsForHost-System-Uri-'></a>
### SettingsForHost() `method`

##### Summary

The ServerConfig which applies to the provided url host part

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Log-Config-Update'></a>
### Update() `method`

##### Summary

Update the Log destinations

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-DataTableForm'></a>
## DataTableForm `type`

##### Namespace

CodeFirstWebFramework

##### Summary

DataTable (jquery) form - always readonly.

<a name='M-CodeFirstWebFramework-DataTableForm-#ctor-CodeFirstWebFramework-AppModule,System-Type-'></a>
### #ctor(module,t) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Creating module |
| t | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type to display in the form |

<a name='M-CodeFirstWebFramework-DataTableForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]-'></a>
### #ctor() `constructor`

##### Summary

DataTable for C# type t with specific fields in specific order

##### Parameters

This constructor has no parameters.

<a name='P-CodeFirstWebFramework-DataTableForm-Select'></a>
### Select `property`

##### Summary

Url to call when the user selects a record.

<a name='M-CodeFirstWebFramework-DataTableForm-RequireField-CodeFirstWebFramework-FieldAttribute-'></a>
### RequireField() `method`

##### Summary

Non-visible fields are included (so you can search on them)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DataTableForm-Show'></a>
### Show() `method`

##### Summary

Render the form to a web page using the appropriate template

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Database'></a>
## Database `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class used for accessing the database.
Programs may subclass this to add more functionality.

<a name='M-CodeFirstWebFramework-Database-#ctor-CodeFirstWebFramework-ServerConfig-'></a>
### #ctor(server) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| server | [CodeFirstWebFramework.ServerConfig](#T-CodeFirstWebFramework-ServerConfig 'CodeFirstWebFramework.ServerConfig') | ServerConfig optionally containing the database type and connection string 
(Config.Default is used for items not supplied) |

<a name='F-CodeFirstWebFramework-Database-Logging'></a>
### Logging `constants`

##### Summary

Whether to log - set to false to suppress logging

<a name='F-CodeFirstWebFramework-Database-Module'></a>
### Module `constants`

##### Summary

The module which created this database

<a name='P-CodeFirstWebFramework-Database-CurrentDbVersion'></a>
### CurrentDbVersion `property`

##### Summary

A database version number stored in the Settings table. Used to check if any extra changes
need to be made on version change.

<a name='P-CodeFirstWebFramework-Database-TableNames'></a>
### TableNames `property`

##### Summary

Return the names of all the tables

<a name='P-CodeFirstWebFramework-Database-UniqueIdentifier'></a>
### UniqueIdentifier `property`

##### Summary

Return a unique identifier based on the connection string so different Database objects
accessing the same database can be recognised as the same.

<a name='P-CodeFirstWebFramework-Database-ViewNames'></a>
### ViewNames `property`

##### Summary

Return the names of all the views

<a name='M-CodeFirstWebFramework-Database-BeginTransaction'></a>
### BeginTransaction() `method`

##### Summary

Start a transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Cast-System-String,System-String-'></a>
### Cast() `method`

##### Summary

Return SQL to cast a value to a type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-CheckValidFieldname-System-String-'></a>
### CheckValidFieldname(f) `method`

##### Summary

Check a field name is valid, throw an exception if not.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| f | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-CodeFirstWebFramework-Database-Clean'></a>
### Clean() `method`

##### Summary

Clean and compact the database.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Commit'></a>
### Commit() `method`

##### Summary

Commit a transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Delete-System-String,Newtonsoft-Json-Linq-JObject-'></a>
### Delete(tableName,data) `method`

##### Summary

Delete a record by content.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| tableName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Table name |
| data | [Newtonsoft.Json.Linq.JObject](#T-Newtonsoft-Json-Linq-JObject 'Newtonsoft.Json.Linq.JObject') | Content - if this matches a unique key, that is the record which will be deleted |

<a name='M-CodeFirstWebFramework-Database-Delete-System-String,System-Int32-'></a>
### Delete() `method`

##### Summary

Delete a record by id.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Delete-CodeFirstWebFramework-JsonObject-'></a>
### Delete(data) `method`

##### Summary

Delete a record by content.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| data | [CodeFirstWebFramework.JsonObject](#T-CodeFirstWebFramework-JsonObject 'CodeFirstWebFramework.JsonObject') | Content - if this matches a unique key, that is the record which will be deleted |

<a name='M-CodeFirstWebFramework-Database-Dispose'></a>
### Dispose() `method`

##### Summary

Dispose of the database.
Any uncommitted transaction will be rolled back, and the connection will be closed.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-EmptyRecord-System-String-'></a>
### EmptyRecord() `method`

##### Summary

Create an empty record for the given table as a JObject

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-EmptyRecord``1'></a>
### EmptyRecord\`\`1() `method`

##### Summary

Create an empty record as a C# object
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Execute-System-String-'></a>
### Execute() `method`

##### Summary

Execute arbitrary SQL

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Exists-System-String,System-Nullable{System-Int32}-'></a>
### Exists() `method`

##### Summary

Find out if a record with the given id exists in the table

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-ForeignKey-System-String,Newtonsoft-Json-Linq-JObject-'></a>
### ForeignKey() `method`

##### Summary

Given data that represents a unique key in a table, if a record matching the key
exists, return its record id, otherwise create a new record using the data and
return its id.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-ForeignKey-System-String,System-Object[]-'></a>
### ForeignKey() `method`

##### Summary

Given name, value pairs that represents a unique key in a table, if a record matching the key
exists, return its record id, otherwise create a new record using the data and
return its id.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Get-System-String,System-Int32-'></a>
### Get() `method`

##### Summary

Get a record by id

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Get``1-System-Int32-'></a>
### Get\`\`1() `method`

##### Summary

Get a record by id
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Get``1-``0-'></a>
### Get\`\`1() `method`

##### Summary

Get a record by unique key
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-In-System-Object[]-'></a>
### In() `method`

##### Summary

Produce an "IN(...)" SQL statement from a list of values

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-In``1-System-Collections-Generic-IEnumerable{``0}-'></a>
### In\`\`1() `method`

##### Summary

Produce an "IN(...)" SQL statement from a list of values

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Insert-System-String,System-Collections-Generic-List{Newtonsoft-Json-Linq-JObject}-'></a>
### Insert() `method`

##### Summary

Insert a series of records into the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Insert-System-String,Newtonsoft-Json-Linq-JObject-'></a>
### Insert() `method`

##### Summary

Insert a record into the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Insert-System-String,CodeFirstWebFramework-JsonObject-'></a>
### Insert() `method`

##### Summary

Insert a record into the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Insert-CodeFirstWebFramework-JsonObject-'></a>
### Insert() `method`

##### Summary

Insert a record into the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-IsValidFieldname-System-String-'></a>
### IsValidFieldname() `method`

##### Summary

Determine if a name is a valid database field name

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-LookupKey-System-String,Newtonsoft-Json-Linq-JObject-'></a>
### LookupKey() `method`

##### Summary

Find the record id of a record given data containing a unique key

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-LookupKey-System-String,System-Object[]-'></a>
### LookupKey() `method`

##### Summary

Find the record id of a record given name, value pairs containing a unique key

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-PostUpgradeFromVersion-System-Int32-'></a>
### PostUpgradeFromVersion(version) `method`

##### Summary

Code that must be run after the database is reconfigured - e.g. populating new fields

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| version | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Original version (-1 = new database) |

<a name='M-CodeFirstWebFramework-Database-PreUpgradeFromVersion-System-Int32-'></a>
### PreUpgradeFromVersion(version) `method`

##### Summary

Code that must be run before the database is reconfigured - e.g. renaming old fields

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| version | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Original version (-1 = new database) |

<a name='M-CodeFirstWebFramework-Database-Query-System-String-'></a>
### Query() `method`

##### Summary

Query the database and return the records as JObjects

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Query-System-String,System-String,System-String[]-'></a>
### Query(fields,conditions,tableNames) `method`

##### Summary

Query the database and return the records as JObjects

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| fields | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Fields to return - leave empty or use "+" to return all relevant fields |
| conditions | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | To use in the WHERE clause (may also include SORT BY) |
| tableNames | [System.String[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String[] 'System.String[]') | List of table names to join for the query. 
Joins are performed automatically on foreign keys. |

<a name='M-CodeFirstWebFramework-Database-QueryOne-System-String-'></a>
### QueryOne() `method`

##### Summary

Query the database and return the first matching record as a JObject (or null if none)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-QueryOne-System-String,System-String,System-String[]-'></a>
### QueryOne() `method`

##### Summary

Query the database and return the first matching record as a JObject (or null if none)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-QueryOne``1-System-String-'></a>
### QueryOne\`\`1() `method`

##### Summary

Query the database and return the first matching record as a C# object (or an empty record if none)
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-QueryOne``1-System-String,System-String,System-String[]-'></a>
### QueryOne\`\`1() `method`

##### Summary

Query the database and return the first matching record as a C# object (or an empty record if none)
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Query``1-System-String-'></a>
### Query\`\`1() `method`

##### Summary

Query the database and return the records as C# objects
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Query``1-System-String,System-String,System-String[]-'></a>
### Query\`\`1(fields,conditions,tableNames) `method`

##### Summary

Query the database and return the records as C# objects
NB If called with T a base class of the class used to create the table, returns an object of the derived class

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| fields | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Fields to return - leave empty or use "+" to return all relevant fields |
| conditions | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | To use in the WHERE clause (may also include SORT BY) |
| tableNames | [System.String[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String[] 'System.String[]') | List of table names to join for the query. 
Joins are performed automatically on foreign keys. |

<a name='M-CodeFirstWebFramework-Database-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote any kind of data for inclusion in a SQL query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-RecordExists-System-String,System-Int32-'></a>
### RecordExists() `method`

##### Summary

Determine if a record with the given id exists

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-RecordExists-CodeFirstWebFramework-Table,System-Int32-'></a>
### RecordExists() `method`

##### Summary

Determine if a record with the given id exists

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Rollback'></a>
### Rollback() `method`

##### Summary

Rollback transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-TableFor-System-String-'></a>
### TableFor() `method`

##### Summary

Find the Table descriptor for a table name

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-TableFor-System-Type-'></a>
### TableFor() `method`

##### Summary

Find the Table descriptor for a C# type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-TableForOrDefault-System-Type-'></a>
### TableForOrDefault() `method`

##### Summary

Try to find the Table descriptor for a C# type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Update-System-String,System-Collections-Generic-List{Newtonsoft-Json-Linq-JObject}-'></a>
### Update() `method`

##### Summary

Update a series of records
If each record doesn't already exist, it will be created.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Update-System-String,Newtonsoft-Json-Linq-JObject-'></a>
### Update() `method`

##### Summary

Update a record.
If the record doesn't already exist, it will be created.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Update-CodeFirstWebFramework-JsonObject-'></a>
### Update() `method`

##### Summary

Update a record.
If the record doesn't already exist, it will be created.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-Upgrade'></a>
### Upgrade() `method`

##### Summary

Check the version in the Settings table. If not the same as CurrentDbVersion, call PreUpgradeFromVersion
In any case, modify all the tables so they match the code
If the version was upgraded, call PostUpgradeFromVersion afterwards,

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-delete-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject-'></a>
### delete(table,data) `method`

##### Summary

Delete a record by content.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table |
| data | [Newtonsoft.Json.Linq.JObject](#T-Newtonsoft-Json-Linq-JObject 'Newtonsoft.Json.Linq.JObject') | Content - if this matches a unique key, that is the record which will be deleted |

<a name='M-CodeFirstWebFramework-Database-update-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject-'></a>
### update() `method`

##### Summary

Update a record.
If the record doesn't already exist, it will be created.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-updateIfChanged-CodeFirstWebFramework-Table,Newtonsoft-Json-Linq-JObject-'></a>
### updateIfChanged() `method`

##### Summary

Update a record only if it has changed.
If the record doesn't already exist, it will be created.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Database-upgrade-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table-'></a>
### upgrade(code,database) `method`

##### Summary

Make an individual table in the database correspond to the code Table class

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| code | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') |  |
| database | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') |  |

<a name='T-CodeFirstWebFramework-DatabaseException'></a>
## DatabaseException `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Exception thrown by database code - contains the SQL and/or the table causing the exception

<a name='M-CodeFirstWebFramework-DatabaseException-#ctor-CodeFirstWebFramework-DatabaseException,CodeFirstWebFramework-Table-'></a>
### #ctor(ex,table) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ex | [CodeFirstWebFramework.DatabaseException](#T-CodeFirstWebFramework-DatabaseException 'CodeFirstWebFramework.DatabaseException') | Exception caught |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table causing the exception |

<a name='M-CodeFirstWebFramework-DatabaseException-#ctor-System-Exception,System-String-'></a>
### #ctor(ex,sql) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| ex | [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') | Exception caught |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL causing the exception |

<a name='F-CodeFirstWebFramework-DatabaseException-Sql'></a>
### Sql `constants`

##### Summary

SQL causing the exception

<a name='F-CodeFirstWebFramework-DatabaseException-Table'></a>
### Table `constants`

##### Summary

Table causing the exception

<a name='P-CodeFirstWebFramework-DatabaseException-Message'></a>
### Message `property`

##### Summary

Message (also shows table causing the exception, if known)

<a name='M-CodeFirstWebFramework-DatabaseException-ToString'></a>
### ToString() `method`

##### Summary

Returns exception string and SQL

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-DbInterface'></a>
## DbInterface `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface through which all interaction between Database and a database-specific implementation happens.

<a name='M-CodeFirstWebFramework-DbInterface-Cast-System-String,System-String-'></a>
### Cast() `method`

##### Summary

Return SQL to cast a value to a type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-CleanDatabase'></a>
### CleanDatabase() `method`

##### Summary

Clean up the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Commit'></a>
### Commit() `method`

##### Summary

Commit current transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Execute-System-String-'></a>
### Execute() `method`

##### Summary

Execute sql, returning id of any record inserted

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field-'></a>
### FieldsMatch() `method`

##### Summary

Do the fields in code and database match (some implementations are case insensitive)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean-'></a>
### Insert() `method`

##### Summary

Insert data in table

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote any kind of data for inclusion in a SQL query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Rollback'></a>
### Rollback() `method`

##### Summary

Rollback transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-Tables'></a>
### Tables() `method`

##### Summary

Dictionary of Tables by name

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DbInterface-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}-'></a>
### UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex) `method`

##### Summary

Upgrade the table definition

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| code | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Defintiion required, from code |
| database | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Definition in database |
| insert | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to insert |
| update | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to change |
| remove | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to remove |
| insertFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to insert |
| dropFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to remove |
| insertIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to insert |
| dropIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to remove |

<a name='M-CodeFirstWebFramework-DbInterface-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View-'></a>
### ViewsMatch() `method`

##### Summary

Do the views in code and database match

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-DecimalFormatJsonConverter'></a>
## DecimalFormatJsonConverter `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Our own converter to/from decimal and float (and decimal? and float?).
Everything is rounded to 4 decimal places to get over floating point inaccuracies.
Converting to object, accepts strings, floats and ints.

<a name='M-CodeFirstWebFramework-DecimalFormatJsonConverter-#ctor'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DecimalFormatJsonConverter-CanConvert-System-Type-'></a>
### CanConvert() `method`

##### Summary

Whether this converter can convert this type of object

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DecimalFormatJsonConverter-ReadJson-Newtonsoft-Json-JsonReader,System-Type,System-Object,Newtonsoft-Json-JsonSerializer-'></a>
### ReadJson() `method`

##### Summary

Reads the object from JSON.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-DecimalFormatJsonConverter-WriteJson-Newtonsoft-Json-JsonWriter,System-Object,Newtonsoft-Json-JsonSerializer-'></a>
### WriteJson() `method`

##### Summary

Writes the JSON representation of the object.

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-DefaultValueAttribute'></a>
## DefaultValueAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Set the default value for a field (on the database)

<a name='M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-Int32-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DefaultValueAttribute-#ctor-System-Boolean-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-DefaultValueAttribute-Value'></a>
### Value `constants`

##### Summary

The default value

<a name='T-CodeFirstWebFramework-Log-Destination'></a>
## Destination `type`

##### Namespace

CodeFirstWebFramework.Log

##### Summary

Log destinations

<a name='F-CodeFirstWebFramework-Log-Destination-Debug'></a>
### Debug `constants`

##### Summary

Log to debug output

<a name='F-CodeFirstWebFramework-Log-Destination-File'></a>
### File `constants`

##### Summary

Log to file (specify with "file:name"

<a name='F-CodeFirstWebFramework-Log-Destination-Log'></a>
### Log `constants`

##### Summary

Log to dated log file in the LogFolder directory

<a name='F-CodeFirstWebFramework-Log-Destination-Null'></a>
### Null `constants`

##### Summary

Do not log

<a name='F-CodeFirstWebFramework-Log-Destination-StdErr'></a>
### StdErr `constants`

##### Summary

Log to stderr

<a name='F-CodeFirstWebFramework-Log-Destination-StdOut'></a>
### StdOut `constants`

##### Summary

Log to stdout

<a name='F-CodeFirstWebFramework-Log-Destination-Trace'></a>
### Trace `constants`

##### Summary

Log to trace output

<a name='T-CodeFirstWebFramework-DirectoryInfo'></a>
## DirectoryInfo `type`

##### Namespace

CodeFirstWebFramework

##### Summary

IDirectoryInfo that uses the filesystem

<a name='M-CodeFirstWebFramework-DirectoryInfo-#ctor-System-String,System-IO-DirectoryInfo-'></a>
### #ctor() `constructor`

##### Summary

Construct from a System.IO.DirectoryInfo

##### Parameters

This constructor has no parameters.

<a name='P-CodeFirstWebFramework-DirectoryInfo-Exists'></a>
### Exists `property`

##### Summary

Whether the directory exists

<a name='P-CodeFirstWebFramework-DirectoryInfo-Name'></a>
### Name `property`

##### Summary

Directory name

<a name='P-CodeFirstWebFramework-DirectoryInfo-Path'></a>
### Path `property`

##### Summary

Path

<a name='M-CodeFirstWebFramework-DirectoryInfo-Content-System-String-'></a>
### Content() `method`

##### Summary

Search the directory for files matching the pattern

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-DoNotStoreAttribute'></a>
## DoNotStoreAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a C# field which is not to be stored in the database

<a name='T-CodeFirstWebFramework-DumbForm'></a>
## DumbForm `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Old-style FORM/SUBMIT form

<a name='M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Boolean-'></a>
### #ctor(module,readwrite) `constructor`

##### Summary

Empty form

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Owning module |
| readwrite | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether the user can input to some of the fields |

<a name='M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type-'></a>
### #ctor() `constructor`

##### Summary

Readwrite form for C# type t

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean-'></a>
### #ctor() `constructor`

##### Summary

Form for C# type t

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DumbForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]-'></a>
### #ctor() `constructor`

##### Summary

Form for C# type t with specific fields in specific order

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-DumbForm-Show'></a>
### Show() `method`

##### Summary

Build the form html from a template. By default it uses /modulename/methodname.tmpl, but, if that doesn't
exist, it uses the default template for the form (/dumbform.tmpl).

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-ErrorModule'></a>
## ErrorModule `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class to show errors

<a name='T-CodeFirstWebFramework-Field'></a>
## Field `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Store details about a database field

<a name='M-CodeFirstWebFramework-Field-#ctor-System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-Field-#ctor-System-String,System-Type,System-Decimal,System-Boolean,System-Boolean,System-String-'></a>
### #ctor(name,type,length,nullable,autoIncrement,defaultValue) `constructor`

##### Summary

Full constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Field name |
| type | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | C# type |
| length | [System.Decimal](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Decimal 'System.Decimal') | Length |
| nullable | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether it may be null |
| autoIncrement | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether it is auto increment |
| defaultValue | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Default value (or null) |

<a name='F-CodeFirstWebFramework-Field-ForeignKey'></a>
### ForeignKey `constants`

##### Summary

Foreign key details (or null)

<a name='P-CodeFirstWebFramework-Field-AutoIncrement'></a>
### AutoIncrement `property`

##### Summary

Whether the field is auto increment

<a name='P-CodeFirstWebFramework-Field-DefaultValue'></a>
### DefaultValue `property`

##### Summary

Default value

<a name='P-CodeFirstWebFramework-Field-Length'></a>
### Length `property`

##### Summary

Length

<a name='P-CodeFirstWebFramework-Field-Name'></a>
### Name `property`

##### Summary

Name

<a name='P-CodeFirstWebFramework-Field-Nullable'></a>
### Nullable `property`

##### Summary

Whether the field may be null

<a name='P-CodeFirstWebFramework-Field-Type'></a>
### Type `property`

##### Summary

C# type

<a name='P-CodeFirstWebFramework-Field-TypeName'></a>
### TypeName `property`

##### Summary

String representation of C# type, allowing for Nullable.
E.g. a Nullable Int32 will retuen "int?"

<a name='M-CodeFirstWebFramework-Field-Data-System-Boolean-'></a>
### Data(view) `method`

##### Summary

String description of Field

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| view | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether this field is in a view |

<a name='M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-FieldInfo,CodeFirstWebFramework-PrimaryAttribute@-'></a>
### FieldFor(field,pk) `method`

##### Summary

Create a Field object from the Attributes on a C# class field (unless it has a DoNotStore attribute)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| field | [System.Reflection.FieldInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.FieldInfo 'System.Reflection.FieldInfo') | The C# FieldInfo for the field |
| pk | [CodeFirstWebFramework.PrimaryAttribute@](#T-CodeFirstWebFramework-PrimaryAttribute@ 'CodeFirstWebFramework.PrimaryAttribute@') | Set to PrimaryAttribute if the field has one |

<a name='M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-FieldInfo-'></a>
### FieldFor(field) `method`

##### Summary

Create a Field object from the Attributes on a C# class field

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| field | [System.Reflection.FieldInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.FieldInfo 'System.Reflection.FieldInfo') | The C# FieldInfo for the field |

<a name='M-CodeFirstWebFramework-Field-FieldFor-System-Reflection-PropertyInfo-'></a>
### FieldFor(field) `method`

##### Summary

Create a Field object from the Attributes on a C# class property

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| field | [System.Reflection.PropertyInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.PropertyInfo 'System.Reflection.PropertyInfo') | The C# PropertyInfo for the field |

<a name='M-CodeFirstWebFramework-Field-FieldFor-System-String,System-Type,System-Boolean,CodeFirstWebFramework-PrimaryAttribute,CodeFirstWebFramework-LengthAttribute,CodeFirstWebFramework-DefaultValueAttribute-'></a>
### FieldFor() `method`

##### Summary

Create a Field object from the Attributes on a C# class field

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Field-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote value of object o to use in a SQL statement, assuming value is to be placed in this field.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Field-ToString'></a>
### ToString() `method`

##### Summary

String representation (for debugging)

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-FieldAttribute'></a>
## FieldAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute to define field display in forms

<a name='M-CodeFirstWebFramework-FieldAttribute-#ctor'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-FieldAttribute-#ctor-System-Object[]-'></a>
### #ctor(args) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | Pairs of name, value, passed direct into Options (so must be javascript style starting with lower case letter) |

<a name='F-CodeFirstWebFramework-FieldAttribute-Field'></a>
### Field `constants`

##### Summary

SQL Field definition

<a name='F-CodeFirstWebFramework-FieldAttribute-Options'></a>
### Options `constants`

##### Summary

The javascript options for the field

<a name='F-CodeFirstWebFramework-FieldAttribute-Types'></a>
### Types `constants`

##### Summary

Pair of types, separated by , - first is read only, second is read-write

<a name='P-CodeFirstWebFramework-FieldAttribute-Attributes'></a>
### Attributes `property`

##### Summary

Html attributes to add to field

<a name='P-CodeFirstWebFramework-FieldAttribute-Colspan'></a>
### Colspan `property`

##### Summary

How many columns for field

<a name='P-CodeFirstWebFramework-FieldAttribute-Data'></a>
### Data `property`

##### Summary

Name of variable containing field value

<a name='P-CodeFirstWebFramework-FieldAttribute-FieldName'></a>
### FieldName `property`

##### Summary

Name of field (allowing for default to Data)

<a name='P-CodeFirstWebFramework-FieldAttribute-Heading'></a>
### Heading `property`

##### Summary

Heading/prompt for field (defaults to Data, un camel cased)

<a name='P-CodeFirstWebFramework-FieldAttribute-MaxLength'></a>
### MaxLength `property`

##### Summary

Number of characters to allow in input

<a name='P-CodeFirstWebFramework-FieldAttribute-Name'></a>
### Name `property`

##### Summary

Name of field - should be unique within a form. Defaults to same as Data.

<a name='P-CodeFirstWebFramework-FieldAttribute-SameRow'></a>
### SameRow `property`

##### Summary

True if field is to be in the same row as the previous field

<a name='P-CodeFirstWebFramework-FieldAttribute-Type'></a>
### Type `property`

##### Summary

Type of field - see list in default.js

<a name='P-CodeFirstWebFramework-FieldAttribute-Visible'></a>
### Visible `property`

##### Summary

Set to false to hide the field (in DataTables) or omit it (in other forms)

<a name='M-CodeFirstWebFramework-FieldAttribute-FieldFor-CodeFirstWebFramework-Database,System-Reflection-FieldInfo,System-Boolean-'></a>
### FieldFor(db,field,readwrite) `method`

##### Summary

Create a FieldAttribute for the given field in a class.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| db | [CodeFirstWebFramework.Database](#T-CodeFirstWebFramework-Database 'CodeFirstWebFramework.Database') | Database (needed to retrieve default select options) |
| field | [System.Reflection.FieldInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.FieldInfo 'System.Reflection.FieldInfo') | FieldInfo definition |
| readwrite | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if the user can edit the field |

<a name='M-CodeFirstWebFramework-FieldAttribute-FieldFor-System-Reflection-PropertyInfo,System-Boolean-'></a>
### FieldFor(field,readwrite) `method`

##### Summary

Create a FieldAttribute for the given property in a class.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| field | [System.Reflection.PropertyInfo](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Reflection.PropertyInfo 'System.Reflection.PropertyInfo') | Property definition |
| readwrite | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if the user can edit the field |

<a name='M-CodeFirstWebFramework-FieldAttribute-MakeSelectable-CodeFirstWebFramework-JObjectEnumerable-'></a>
### MakeSelectable() `method`

##### Summary

Turn a field into a select or selectInput

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-FieldAttribute-MakeSelectable-System-Collections-Generic-IEnumerable{Newtonsoft-Json-Linq-JObject}-'></a>
### MakeSelectable() `method`

##### Summary

Turn a field into a select or selectInput

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-FileInfo'></a>
## FileInfo `type`

##### Namespace

CodeFirstWebFramework

##### Summary

IFileInfo that uses the filesystem

<a name='M-CodeFirstWebFramework-FileInfo-#ctor-System-String,System-IO-FileInfo-'></a>
### #ctor() `constructor`

##### Summary

Construct from a System.IO.FileInfo

##### Parameters

This constructor has no parameters.

<a name='P-CodeFirstWebFramework-FileInfo-Exists'></a>
### Exists `property`

##### Summary

Whether the file exists

<a name='P-CodeFirstWebFramework-FileInfo-Extension'></a>
### Extension `property`

##### Summary

File extension

<a name='P-CodeFirstWebFramework-FileInfo-LastWriteTimeUtc'></a>
### LastWriteTimeUtc `property`

##### Summary

The modification time

<a name='P-CodeFirstWebFramework-FileInfo-Name'></a>
### Name `property`

##### Summary

File name without extension

<a name='P-CodeFirstWebFramework-FileInfo-Path'></a>
### Path `property`

##### Summary

Path

<a name='M-CodeFirstWebFramework-FileInfo-Content-CodeFirstWebFramework-AppModule-'></a>
### Content() `method`

##### Summary

File content

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-FileInfo-Stream-CodeFirstWebFramework-AppModule-'></a>
### Stream() `method`

##### Summary

Stream containing content

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-FileSender'></a>
## FileSender `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class to serve files.
This is used if there is no AppModule corresponding to the directory in the web request.
If an html file is requested, it does not exist, but there is a corresponding tmpl file, the template is filled in and returned.

<a name='M-CodeFirstWebFramework-FileSender-#ctor-System-String-'></a>
### #ctor() `constructor`

##### Summary

Create a FileSender for a file

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-FileSender-ContentTypes'></a>
### ContentTypes `constants`

##### Summary

Dictionary to translate file extensions to content types in file responses.
Add entries to this if you have your own mime types.

<a name='F-CodeFirstWebFramework-FileSender-Filename'></a>
### Filename `constants`

##### Summary

The name of the file (as a url)

<a name='M-CodeFirstWebFramework-FileSender-Default'></a>
### Default() `method`

##### Summary

Default behaviour is to return the file contents, processing .tmpl and .md files as appropriate.

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-FileSystem'></a>
## FileSystem `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface to the file system which converts file names into IFileInfo objects,
and directory names into IDirectoryInfo objects

<a name='M-CodeFirstWebFramework-FileSystem-DirectoryInfo-CodeFirstWebFramework-AppModule,System-String-'></a>
### DirectoryInfo(module,foldername) `method`

##### Summary

Search the list of folders for a folder matching the foldername

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | AppModule making the call |
| foldername | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Like a url - e.g. "admin/settings.html" |

<a name='M-CodeFirstWebFramework-FileSystem-FileInfo-CodeFirstWebFramework-AppModule,System-String-'></a>
### FileInfo(module,filename) `method`

##### Summary

Search the list of folders for a file matching the filename

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | AppModule making the call |
| filename | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Like a url - e.g. "admin/settings.html" |

<a name='T-CodeFirstWebFramework-ForeignKey'></a>
## ForeignKey `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Store details about a Foreign Key field

<a name='M-CodeFirstWebFramework-ForeignKey-#ctor-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field-'></a>
### #ctor(table,field) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table the key refers to |
| field | [CodeFirstWebFramework.Field](#T-CodeFirstWebFramework-Field 'CodeFirstWebFramework.Field') | Field in that table |

<a name='P-CodeFirstWebFramework-ForeignKey-Field'></a>
### Field `property`

##### Summary

Field in that table

<a name='P-CodeFirstWebFramework-ForeignKey-Table'></a>
### Table `property`

##### Summary

Table the key refers to

<a name='T-CodeFirstWebFramework-ForeignKeyAttribute'></a>
## ForeignKeyAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute marking a field as a foreign key

<a name='M-CodeFirstWebFramework-ForeignKeyAttribute-#ctor-System-String-'></a>
### #ctor(table) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The foreign table |

<a name='P-CodeFirstWebFramework-ForeignKeyAttribute-Table'></a>
### Table `property`

##### Summary

The foreign table

<a name='T-CodeFirstWebFramework-Form'></a>
## Form `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Normal input form.

<a name='M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Boolean-'></a>
### #ctor(module,readwrite) `constructor`

##### Summary

Empty form

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Owning module |
| readwrite | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether the user can input to some of the fields |

<a name='M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type-'></a>
### #ctor() `constructor`

##### Summary

Readwrite form for C# type t

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean-'></a>
### #ctor() `constructor`

##### Summary

Form for C# type t

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-Form-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]-'></a>
### #ctor() `constructor`

##### Summary

Form for C# type t with specific fields in specific order

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-Form-ReadWrite'></a>
### ReadWrite `constants`

##### Summary

Whether the user can input

<a name='F-CodeFirstWebFramework-Form-columns'></a>
### columns `constants`

##### Summary

The fields

<a name='P-CodeFirstWebFramework-Form-CanDelete'></a>
### CanDelete `property`

##### Summary

Whether the user can delete record.

<a name='P-CodeFirstWebFramework-Form-Fields'></a>
### Fields `property`

##### Summary

All the fields in this form

<a name='P-CodeFirstWebFramework-Form-Item-System-String-'></a>
### Item `property`

##### Summary

Find a field by name

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-CodeFirstWebFramework-Form-Add-System-Reflection-FieldInfo-'></a>
### Add() `method`

##### Summary

Add a field from a C# class to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Add-System-Reflection-FieldInfo,System-Boolean-'></a>
### Add() `method`

##### Summary

Add a field from a C# class to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Add-CodeFirstWebFramework-FieldAttribute-'></a>
### Add() `method`

##### Summary

Add a field to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Add-System-Type,System-String-'></a>
### Add() `method`

##### Summary

Add a field to the form by name

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Build-System-Type-'></a>
### Build() `method`

##### Summary

Add all the suitable fields from a C# type to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-IndexOf-System-String-'></a>
### IndexOf() `method`

##### Summary

Return the index of the named field in the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Insert-System-Int32,CodeFirstWebFramework-FieldAttribute-'></a>
### Insert() `method`

##### Summary

Insert a field from a C# class to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Remove-System-String-'></a>
### Remove() `method`

##### Summary

Remove the named field

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Remove-System-String[]-'></a>
### Remove() `method`

##### Summary

Remove the named fields

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Replace-System-Int32,CodeFirstWebFramework-FieldAttribute-'></a>
### Replace() `method`

##### Summary

Replace a field from a C# class to the form

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-RequireField-CodeFirstWebFramework-FieldAttribute-'></a>
### RequireField() `method`

##### Summary

Decide whether a field should be included - e.g. autoincrement and non-visible fields are excluded by default.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-Show'></a>
### Show() `method`

##### Summary

Render the form to the web page, using the appropriate template

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Form-processFields-System-Type,System-Boolean-'></a>
### processFields(tbl,inTable) `method`

##### Summary

Process all the fields from a type (do any base classes first)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| tbl | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type being analysed |
| inTable | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if this is a base class of a Table or Writeable object |

<a name='T-CodeFirstWebFramework-HandlesAttribute'></a>
## HandlesAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute to indicate what file extensions a module can handle

<a name='M-CodeFirstWebFramework-HandlesAttribute-#ctor-System-String[]-'></a>
### #ctor() `constructor`

##### Summary

Attribute to indicate which extensions an AppModule can handle

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-HandlesAttribute-Extensions'></a>
### Extensions `constants`

##### Summary

List of extensions this AppModule can handle

<a name='T-CodeFirstWebFramework-HeaderDetailForm'></a>
## HeaderDetailForm `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Header detailt form

<a name='M-CodeFirstWebFramework-HeaderDetailForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Type-'></a>
### #ctor(module,header,detail) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Owning module |
| header | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | C# type in the header |
| detail | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | C# type in the detail |

<a name='M-CodeFirstWebFramework-HeaderDetailForm-#ctor-CodeFirstWebFramework-AppModule,CodeFirstWebFramework-Form,CodeFirstWebFramework-ListForm-'></a>
### #ctor(module,header,detail) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Owning module |
| header | [CodeFirstWebFramework.Form](#T-CodeFirstWebFramework-Form 'CodeFirstWebFramework.Form') | Header form |
| detail | [CodeFirstWebFramework.ListForm](#T-CodeFirstWebFramework-ListForm 'CodeFirstWebFramework.ListForm') | Detail form |

<a name='F-CodeFirstWebFramework-HeaderDetailForm-Detail'></a>
### Detail `constants`

##### Summary

The Detail ListForm

<a name='F-CodeFirstWebFramework-HeaderDetailForm-Header'></a>
### Header `constants`

##### Summary

The header Form

<a name='P-CodeFirstWebFramework-HeaderDetailForm-CanDelete'></a>
### CanDelete `property`

##### Summary

Whether the user can delete record.

<a name='M-CodeFirstWebFramework-HeaderDetailForm-Show'></a>
### Show() `method`

##### Summary

Render the form to a web page using the appropriate template

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Help'></a>
## Help `type`

##### Namespace

CodeFirstWebFramework

##### Summary

AppModule to handle help requests

<a name='F-CodeFirstWebFramework-Help-Contents'></a>
### Contents `constants`

##### Summary

Whether the help has a table of contents (used in templates)

<a name='F-CodeFirstWebFramework-Help-Next'></a>
### Next `constants`

##### Summary

Markdown-style link to next page (if any) in table of contents

<a name='F-CodeFirstWebFramework-Help-Parent'></a>
### Parent `constants`

##### Summary

Markdown-style link to parent (if any) in table of contents

<a name='F-CodeFirstWebFramework-Help-Previous'></a>
### Previous `constants`

##### Summary

Markdown-style link to previous page (if any) in table of contents

<a name='M-CodeFirstWebFramework-Help-CallMethod-System-Reflection-MethodInfo@-'></a>
### CallMethod() `method`

##### Summary

Override CallMethod so it also accepts unknown Method names (presuming them to be md files) and still displays them

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Help-Default'></a>
### Default() `method`

##### Summary

Display a help file from the url

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Help-LoadHelpFrom-CodeFirstWebFramework-IFileInfo-'></a>
### LoadHelpFrom() `method`

##### Summary

Return the Markdown help text from file in a web page ready to render.
If Method is not "default.md", load the table of contents information
If the file is itself a template, fill it in first (output will be Markdown).
Renders the md file inside the "/help/default.tmpl" template (to add the contents links)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Help-ReturnHelpFrom-CodeFirstWebFramework-IFileInfo-'></a>
### ReturnHelpFrom() `method`

##### Summary

Render the Markdown help text from file in a web page.
Allows caching and supports If-Modified-Since headers
If Method is not "default.md", load the table of contents information
If the file is itself a template, fill it in first (output will be Markdown).
Renders the md file inside the "/help/default.tmpl" template (to add the contents links)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Help-parseContents-System-String-'></a>
### parseContents(current) `method`

##### Summary

Set up Next, Previous and Parent by finding the current file in the default.md table of contents

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| current | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Current file |

<a name='T-CodeFirstWebFramework-IDirectoryInfo'></a>
## IDirectoryInfo `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface to "directories" the web server can access.
An interface is used so a Namespace can deliver content from the database instead of the file system

<a name='P-CodeFirstWebFramework-IDirectoryInfo-Exists'></a>
### Exists `property`

##### Summary

Whether the directory exists

<a name='P-CodeFirstWebFramework-IDirectoryInfo-Name'></a>
### Name `property`

##### Summary

Directory name

<a name='P-CodeFirstWebFramework-IDirectoryInfo-Path'></a>
### Path `property`

##### Summary

Path

<a name='M-CodeFirstWebFramework-IDirectoryInfo-Content-System-String-'></a>
### Content() `method`

##### Summary

List of IFileInfo items in the directory

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-IFileInfo'></a>
## IFileInfo `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface to "files" the web server can access.
An interface is used so a Namespace can deliver content from the database instead of the file system

<a name='P-CodeFirstWebFramework-IFileInfo-Exists'></a>
### Exists `property`

##### Summary

Whether the file exists

<a name='P-CodeFirstWebFramework-IFileInfo-Extension'></a>
### Extension `property`

##### Summary

File extension

<a name='P-CodeFirstWebFramework-IFileInfo-LastWriteTimeUtc'></a>
### LastWriteTimeUtc `property`

##### Summary

The modification time

<a name='P-CodeFirstWebFramework-IFileInfo-Name'></a>
### Name `property`

##### Summary

File name (without extension)

<a name='P-CodeFirstWebFramework-IFileInfo-Path'></a>
### Path `property`

##### Summary

Path

<a name='M-CodeFirstWebFramework-IFileInfo-Content-CodeFirstWebFramework-AppModule-'></a>
### Content() `method`

##### Summary

Text of the file

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-IFileInfo-Stream-CodeFirstWebFramework-AppModule-'></a>
### Stream() `method`

##### Summary

Stream containing content

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-ImplementationAttribute'></a>
## ImplementationAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute which indicates an AppModule class uses a helper class 
to implement some or all of its methods.
The helper class must have a constructor which takes a single AppModule parameter.

<a name='M-CodeFirstWebFramework-ImplementationAttribute-#ctor-System-Type-'></a>
### #ctor(helperClass) `constructor`

##### Summary

Constructor.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| helperClass | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type of helper class. 
It must have a constructor which takes a single AppModule parameter. |

<a name='P-CodeFirstWebFramework-ImplementationAttribute-Helper'></a>
### Helper `property`

##### Summary

The helper class type.

<a name='T-CodeFirstWebFramework-Index'></a>
## Index `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Index descriptor

<a name='M-CodeFirstWebFramework-Index-#ctor-System-String,CodeFirstWebFramework-Field[]-'></a>
### #ctor(name,fields) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Index name |
| fields | [CodeFirstWebFramework.Field[]](#T-CodeFirstWebFramework-Field[] 'CodeFirstWebFramework.Field[]') | Fields making up the index |

<a name='M-CodeFirstWebFramework-Index-#ctor-System-String,System-String[]-'></a>
### #ctor(name,fields) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Index name |
| fields | [System.String[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String[] 'System.String[]') | Field names making up the index |

<a name='P-CodeFirstWebFramework-Index-FieldList'></a>
### FieldList `property`

##### Summary

List of fields (with types) in the index, separate by commas - for matching two indexes to see if they are the same

<a name='P-CodeFirstWebFramework-Index-Fields'></a>
### Fields `property`

##### Summary

The fields that go to make up the index

<a name='P-CodeFirstWebFramework-Index-Name'></a>
### Name `property`

##### Summary

Index name

<a name='M-CodeFirstWebFramework-Index-CoversData-Newtonsoft-Json-Linq-JObject-'></a>
### CoversData() `method`

##### Summary

Whether this index has values in the data for all of its fields

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Index-ToString'></a>
### ToString() `method`

##### Summary

For debugging

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Index-Where-Newtonsoft-Json-Linq-JObject-'></a>
### Where() `method`

##### Summary

Generate a WHERE clause (without the "WHERE") to select the record matching data for this index

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-JObjectEnumerable'></a>
## JObjectEnumerable `type`

##### Namespace

CodeFirstWebFramework

##### Summary

An enumerable of JObjects with a JArray converter, to efficiently handle output of a query

<a name='M-CodeFirstWebFramework-JObjectEnumerable-#ctor-System-Collections-Generic-IEnumerable{Newtonsoft-Json-Linq-JObject}-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-JObjectEnumerable-GetEnumerator'></a>
### GetEnumerator() `method`

##### Summary

Standard GetEnumerator

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JObjectEnumerable-System#Collections#IEnumerable#GetEnumerator'></a>
### System#Collections#IEnumerable#GetEnumerator() `method`

##### Summary

Standard GetEnumerator

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JObjectEnumerable-ToList'></a>
### ToList() `method`

##### Summary

ToList converter. If called, the enumerable itself is converted to a list, so that it won't be enumerated again.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JObjectEnumerable-ToString'></a>
### ToString() `method`

##### Summary

For debugging

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JObjectEnumerable-op_Implicit-CodeFirstWebFramework-JObjectEnumerable-~Newtonsoft-Json-Linq-JArray'></a>
### op_Implicit() `method`

##### Summary

Convert to a JArray

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-JsonObject'></a>
## JsonObject `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Base class for all [Table] C# objects

<a name='P-CodeFirstWebFramework-JsonObject-Id'></a>
### Id `property`

##### Summary

Record id

<a name='M-CodeFirstWebFramework-JsonObject-Clone``1'></a>
### Clone\`\`1() `method`

##### Summary

Make a copy

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JsonObject-ToJObject'></a>
### ToJObject() `method`

##### Summary

Convert to a JObject

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-JsonObject-ToString'></a>
### ToString() `method`

##### Summary

For debugging

##### Returns



##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-LengthAttribute'></a>
## LengthAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Set the length (and optionally precision) of a field in the database

<a name='M-CodeFirstWebFramework-LengthAttribute-#ctor-System-Int32-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-LengthAttribute-#ctor-System-Int32,System-Int32-'></a>
### #ctor() `constructor`

##### Summary

Constructor (with precision)

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-LengthAttribute-Length'></a>
### Length `constants`

##### Summary

Field length

<a name='F-CodeFirstWebFramework-LengthAttribute-Precision'></a>
### Precision `constants`

##### Summary

Field precision (if required)

<a name='T-CodeFirstWebFramework-ListForm'></a>
## ListForm `type`

##### Namespace

CodeFirstWebFramework

##### Summary

List form

<a name='M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type-'></a>
### #ctor(module,t) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Creating module |
| t | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type to display in the list |

<a name='M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean-'></a>
### #ctor(module,t,readWrite) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| module | [CodeFirstWebFramework.AppModule](#T-CodeFirstWebFramework-AppModule 'CodeFirstWebFramework.AppModule') | Creating module |
| t | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') | Type to display in the list |
| readWrite | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Whether the user can update the data |

<a name='M-CodeFirstWebFramework-ListForm-#ctor-CodeFirstWebFramework-AppModule,System-Type,System-Boolean,System-String[]-'></a>
### #ctor() `constructor`

##### Summary

Constructor for C# type t with specific fields in specific order

##### Parameters

This constructor has no parameters.

<a name='P-CodeFirstWebFramework-ListForm-Select'></a>
### Select `property`

##### Summary

Url to call when the user selects a record.

<a name='M-CodeFirstWebFramework-ListForm-Show'></a>
### Show() `method`

##### Summary

Render the form to a web page using the appropriate template

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Log'></a>
## Log `type`

##### Namespace

CodeFirstWebFramework

##### Summary

A class to manage logging to a variety of destinations

<a name='M-CodeFirstWebFramework-Log-#ctor-CodeFirstWebFramework-Log-Destination-'></a>
### #ctor() `constructor`

##### Summary

Create Log with specific destination

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-Log-#ctor-System-String-'></a>
### #ctor(config) `constructor`

##### Summary

Parse config for log info and create Log accordingly

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| config | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='F-CodeFirstWebFramework-Log-DatabaseRead'></a>
### DatabaseRead `constants`

##### Summary

Database access logging

<a name='F-CodeFirstWebFramework-Log-DatabaseWrite'></a>
### DatabaseWrite `constants`

##### Summary

Database write logging

<a name='F-CodeFirstWebFramework-Log-Debug'></a>
### Debug `constants`

##### Summary

Debug logging

<a name='F-CodeFirstWebFramework-Log-Error'></a>
### Error `constants`

##### Summary

Exception and error logging

<a name='F-CodeFirstWebFramework-Log-Info'></a>
### Info `constants`

##### Summary

Ordinary web request logging (like access.log)

<a name='F-CodeFirstWebFramework-Log-LogFolder'></a>
### LogFolder `constants`

##### Summary

Folder in which to put dated log files

<a name='F-CodeFirstWebFramework-Log-NotFound'></a>
### NotFound `constants`

##### Summary

Web request which failed logging

<a name='F-CodeFirstWebFramework-Log-PostData'></a>
### PostData `constants`

##### Summary

Post data logging

<a name='F-CodeFirstWebFramework-Log-Session'></a>
### Session `constants`

##### Summary

Session logging

<a name='F-CodeFirstWebFramework-Log-Startup'></a>
### Startup `constants`

##### Summary

Program startup logging

<a name='F-CodeFirstWebFramework-Log-Trace'></a>
### Trace `constants`

##### Summary

Trace logging

<a name='P-CodeFirstWebFramework-Log-On'></a>
### On `property`

##### Summary

Whether this type of logging is outputting anywhere

<a name='M-CodeFirstWebFramework-Log-Close'></a>
### Close() `method`

##### Summary

Close the file

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Log-Flush'></a>
### Flush() `method`

##### Summary

Flush the file to disk

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Log-WriteLine-System-String-'></a>
### WriteLine() `method`

##### Summary

Log message to console and trace

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Log-WriteLine-System-String,System-Object[]-'></a>
### WriteLine() `method`

##### Summary

Log message to console and trace

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-MenuOption'></a>
## MenuOption `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Menu option for the second level menu

<a name='M-CodeFirstWebFramework-MenuOption-#ctor-System-String,System-String-'></a>
### #ctor(text,url) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Menu text |
| url | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Url to go to |

<a name='M-CodeFirstWebFramework-MenuOption-#ctor-System-String,System-String,System-Boolean-'></a>
### #ctor(text,url,enabled) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| text | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Menu text |
| url | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Url to go to |
| enabled | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | False if disabled |

<a name='F-CodeFirstWebFramework-MenuOption-Enabled'></a>
### Enabled `constants`

##### Summary

Is the option enabled

<a name='F-CodeFirstWebFramework-MenuOption-Text'></a>
### Text `constants`

##### Summary

Menu text

<a name='F-CodeFirstWebFramework-MenuOption-Url'></a>
### Url `constants`

##### Summary

Url to go to

<a name='P-CodeFirstWebFramework-MenuOption-Disabled'></a>
### Disabled `property`

##### Summary

Is the option disabled (used in Mustache templates)

<a name='P-CodeFirstWebFramework-MenuOption-Id'></a>
### Id `property`

##### Summary

Html element id - text with no spaces

<a name='T-CodeFirstWebFramework-ModuleInfo'></a>
## ModuleInfo `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class to hold a module name, for use in templates

<a name='M-CodeFirstWebFramework-ModuleInfo-#ctor-System-String,System-Type-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-ModuleInfo-Auth'></a>
### Auth `constants`

##### Summary

The AuthAttribute associated with this module (or one with Any access if there is none)

<a name='F-CodeFirstWebFramework-ModuleInfo-AuthMethods'></a>
### AuthMethods `constants`

##### Summary

Dictionary of method names that have an Auth attribute

<a name='F-CodeFirstWebFramework-ModuleInfo-ModuleAccessLevel'></a>
### ModuleAccessLevel `constants`

##### Summary

Auth access level (or AccessLevel.Any)

<a name='F-CodeFirstWebFramework-ModuleInfo-Name'></a>
### Name `constants`

##### Summary

Name of module

<a name='F-CodeFirstWebFramework-ModuleInfo-Type'></a>
### Type `constants`

##### Summary

AppModule type

<a name='P-CodeFirstWebFramework-ModuleInfo-LowestAccessLevel'></a>
### LowestAccessLevel `property`

##### Summary

Lowest Access level for any method.
Returns AccessLevel.Any if all methods have that level (or there are none),
otherwise the lowest level > Any

<a name='P-CodeFirstWebFramework-ModuleInfo-UnCamelName'></a>
### UnCamelName `property`

##### Summary

Uncamelled name for display

<a name='M-CodeFirstWebFramework-ModuleInfo-addMethods-System-Type-'></a>
### addMethods(t) `method`

##### Summary

Look for Auth attributes on all the methods of a type, and add them to the dictionary

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| t | [System.Type](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Type 'System.Type') |  |

<a name='T-CodeFirstWebFramework-MySqlDatabase'></a>
## MySqlDatabase `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface to MySql

<a name='M-CodeFirstWebFramework-MySqlDatabase-#ctor-CodeFirstWebFramework-Database,System-String-'></a>
### #ctor(db,connectionString) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| db | [CodeFirstWebFramework.Database](#T-CodeFirstWebFramework-Database 'CodeFirstWebFramework.Database') |  |
| connectionString | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='M-CodeFirstWebFramework-MySqlDatabase-BeginTransaction'></a>
### BeginTransaction() `method`

##### Summary

Begin transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Cast-System-String,System-String-'></a>
### Cast() `method`

##### Summary

Return SQL to cast a value to a type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-CleanDatabase'></a>
### CleanDatabase() `method`

##### Summary

Clean up database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Commit'></a>
### Commit() `method`

##### Summary

Commit transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### CreateIndex() `method`

##### Summary

Create an index from a table and index definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-CreateTable-CodeFirstWebFramework-Table-'></a>
### CreateTable() `method`

##### Summary

Create a table from a Table definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Dispose'></a>
### Dispose() `method`

##### Summary

Roll back any uncommitted transaction and close the connection

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### DropIndex() `method`

##### Summary

Drop an index

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-DropTable-CodeFirstWebFramework-Table-'></a>
### DropTable() `method`

##### Summary

Drop a table

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Execute-System-String-'></a>
### Execute() `method`

##### Summary

Execute arbitrary sql

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field-'></a>
### FieldsMatch() `method`

##### Summary

Determine whether two fields are the same

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean-'></a>
### Insert(table,sql,updatesAutoIncrement) `method`

##### Summary

Insert a record

##### Returns

The value of the auto-increment record id of the newly inserted record

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL INSERT statement |
| updatesAutoIncrement | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if the insert may update an auto-increment field |

<a name='M-CodeFirstWebFramework-MySqlDatabase-Query-System-String-'></a>
### Query() `method`

##### Summary

Query the database, and return JObjects for each record returned

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-QueryOne-System-String-'></a>
### QueryOne() `method`

##### Summary

Query the database, and return the first record matching the query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote any kind of data for inclusion in a SQL query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Rollback'></a>
### Rollback() `method`

##### Summary

Rollback transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-Tables'></a>
### Tables() `method`

##### Summary

Get a Dictionary of existing tables in the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-MySqlDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}-'></a>
### UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex) `method`

##### Summary

Upgrade the table definition

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| code | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Defintiion required, from code |
| database | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Definition in database |
| insert | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to insert |
| update | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to change |
| remove | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to remove |
| insertFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to insert |
| dropFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to remove |
| insertIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to insert |
| dropIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to remove |

<a name='M-CodeFirstWebFramework-MySqlDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View-'></a>
### ViewsMatch() `method`

##### Summary

Determine whether two views are the same

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Namespace'></a>
## Namespace `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Desribes a namespace with AppModules which makes up a WebApp

<a name='M-CodeFirstWebFramework-Namespace-#ctor-CodeFirstWebFramework-ServerConfig-'></a>
### #ctor() `constructor`

##### Summary

Constructor - uses reflection to get the information

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-Namespace-FileSystem'></a>
### FileSystem `constants`

##### Summary

The FileSystem for this Namespace

<a name='P-CodeFirstWebFramework-Namespace-EmptySession'></a>
### EmptySession `property`

##### Summary

Create an empty Session object of the appropriate type for this Namespace

<a name='P-CodeFirstWebFramework-Namespace-Modules'></a>
### Modules `property`

##### Summary

List of module names for templates (e.g. to auto-generate a module menu)

<a name='P-CodeFirstWebFramework-Namespace-Name'></a>
### Name `property`

##### Summary

Namespace name

<a name='P-CodeFirstWebFramework-Namespace-TableNames'></a>
### TableNames `property`

##### Summary

List of the table names

<a name='P-CodeFirstWebFramework-Namespace-Tables'></a>
### Tables `property`

##### Summary

Dictionary of tables defined in this namespace

<a name='P-CodeFirstWebFramework-Namespace-ViewNames'></a>
### ViewNames `property`

##### Summary

List of the view names

<a name='M-CodeFirstWebFramework-Namespace-Create-CodeFirstWebFramework-ServerConfig-'></a>
### Create(server) `method`

##### Summary

Create a Namespace object for the server.
Looks for a class called "Namespace" in the server's Namespace which is a subclass of Namespace, 
and has a constructor accepting a single ServerConfig argument.
If not found, creates a base Namespace object

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| server | [CodeFirstWebFramework.ServerConfig](#T-CodeFirstWebFramework-ServerConfig 'CodeFirstWebFramework.ServerConfig') |  |

<a name='M-CodeFirstWebFramework-Namespace-GetAccessLevel'></a>
### GetAccessLevel() `method`

##### Summary

Returns the AccessLevel object to use.
If there is one in the namespace, returns an instance of that, otherwise an instance of CodeFirstWebFramework.AccessLevel

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-GetDatabase-CodeFirstWebFramework-ServerConfig-'></a>
### GetDatabase(server) `method`

##### Summary

Returns the Database object to use.
If there is one in the namespace, returns an instance of that, otherwise an instance of CodeFirstWebFramework.Database

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| server | [CodeFirstWebFramework.ServerConfig](#T-CodeFirstWebFramework-ServerConfig 'CodeFirstWebFramework.ServerConfig') | ConfigServer to pass to the database constructor |

<a name='M-CodeFirstWebFramework-Namespace-GetInstanceOf``1-System-Object[]-'></a>
### GetInstanceOf\`\`1(args) `method`

##### Summary

If there is a subclass of T in the namespace, create an instance of it.
Otherwise create an instance of T

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| args | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The constructor arguments |

##### Generic Types

| Name | Description |
| ---- | ----------- |
| T | The class to create |

<a name='M-CodeFirstWebFramework-Namespace-GetModuleInfo-System-String-'></a>
### GetModuleInfo() `method`

##### Summary

Get the AppModule for a module name from the url

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-GetNamespaceType-System-Type-'></a>
### GetNamespaceType() `method`

##### Summary

If there is a subclass of baseType in the namespace, returns that, otherwise baseType

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-ParseUri-System-String,System-String@-'></a>
### ParseUri() `method`

##### Summary

Parse a uri and return the ModuleInfo associated with it (or null if none).
Sets filename to the proper relative filename (modulename/methodname.extension), stripping off VersionSuffix, 
and adding any defaults (home/default if uri is "/", for instance).

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-TableFor-System-String-'></a>
### TableFor() `method`

##### Summary

Find a Table object by name - throw if not found

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-processFields-System-Type,System-Collections-Generic-List{CodeFirstWebFramework-Field}@,System-Collections-Generic-Dictionary{System-String,System-Collections-Generic-List{System-Tuple{System-Int32,CodeFirstWebFramework-Field}}}@,System-Collections-Generic-List{System-Tuple{System-Int32,CodeFirstWebFramework-Field}}@,System-String@-'></a>
### processFields() `method`

##### Summary

Update the field, index, etc. information for a C# type.
Process base classes first.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Namespace-processTable-System-Type,CodeFirstWebFramework-ViewAttribute-'></a>
### processTable() `method`

##### Summary

Generate Table or View object for a C# class

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-NullableAttribute'></a>
## NullableAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a field as allowed to be null

<a name='T-CodeFirstWebFramework-Permission'></a>
## Permission `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Module level permissions for a user

<a name='F-CodeFirstWebFramework-Permission-FunctionAccessLevel'></a>
### FunctionAccessLevel `constants`

##### Summary

The AccessLevel granted to this user for this module/method.

<a name='F-CodeFirstWebFramework-Permission-Method'></a>
### Method `constants`

##### Summary

Method name, or Name from method level AuthAttribute if there is one specified.
"-" for a module-level Permission.
Multiple method-level AuthAttributes with the same name have the same Permission record.

<a name='F-CodeFirstWebFramework-Permission-MinAccessLevel'></a>
### MinAccessLevel `constants`

##### Summary

Min access level needed for a user to be able to access this module/method.
For example. for a module, the lowest access level of all the methods (not counting
AccessLevel.Any), or the module access level, whichever is the less.
Used in the UI to remove irrelevant access levels from the drop-down list.

<a name='F-CodeFirstWebFramework-Permission-Module'></a>
### Module `constants`

##### Summary

Module name, or Name from module level AuthAttribute if there is one specified.
Multiple module-level AuthAttributes with the same name have the same Permission record.

<a name='F-CodeFirstWebFramework-Permission-UserId'></a>
### UserId `constants`

##### Summary

User to whom this Permission applies.

<a name='P-CodeFirstWebFramework-Permission-Function'></a>
### Function `property`

##### Summary

Show method name in human-readable format.

<a name='T-CodeFirstWebFramework-PrimaryAttribute'></a>
## PrimaryAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a field as the primary key

<a name='M-CodeFirstWebFramework-PrimaryAttribute-#ctor'></a>
### #ctor() `constructor`

##### Summary

Constructor - sets AutoIncrement to true by default, and Name to "PRIMARY"

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-PrimaryAttribute-#ctor-System-Int32-'></a>
### #ctor(sequence) `constructor`

##### Summary

Constructor for when multiple fields make up the key

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sequence | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Which order the fields are in |

<a name='F-CodeFirstWebFramework-PrimaryAttribute-AutoIncrement'></a>
### AutoIncrement `constants`

##### Summary

Whether the key is AutoIncrement (should only be applied to integer keys, usually the record id)

<a name='P-CodeFirstWebFramework-PrimaryAttribute-Name'></a>
### Name `property`

##### Summary

Key Name ("PRIMARY" by default)

<a name='P-CodeFirstWebFramework-PrimaryAttribute-Sequence'></a>
### Sequence `property`

##### Summary

Sequence when multiple fields make up the index

<a name='T-CodeFirstWebFramework-ReadOnlyAttribute'></a>
## ReadOnlyAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Indicate a field or class is readonly by default, even if it is part of a Table

<a name='T-CodeFirstWebFramework-SQLiteConcat'></a>
## SQLiteConcat `type`

##### Namespace

CodeFirstWebFramework

##### Summary

CONCAT function - just like MySql

<a name='T-CodeFirstWebFramework-SQLiteDatabase'></a>
## SQLiteDatabase `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Interface to SQLite

<a name='M-CodeFirstWebFramework-SQLiteDatabase-#ctor-CodeFirstWebFramework-Database,System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-#cctor'></a>
### #cctor() `method`

##### Summary

Static constructor registers the extension functions to make SQLite more like MySql

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-BeginTransaction'></a>
### BeginTransaction() `method`

##### Summary

Begin transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Cast-System-String,System-String-'></a>
### Cast() `method`

##### Summary

Return SQL to cast a value to a type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-CleanDatabase'></a>
### CleanDatabase() `method`

##### Summary

Clean up database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Commit'></a>
### Commit() `method`

##### Summary

Commit transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### CreateIndex() `method`

##### Summary

Create an index from a table and index definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-CreateTable-CodeFirstWebFramework-Table-'></a>
### CreateTable() `method`

##### Summary

Create a table from a Table definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Dispose'></a>
### Dispose() `method`

##### Summary

Roll back any uncommitted transaction and close the connection

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### DropIndex() `method`

##### Summary

Drop an index

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-DropTable-CodeFirstWebFramework-Table-'></a>
### DropTable() `method`

##### Summary

Drop a table

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Execute-System-String-'></a>
### Execute() `method`

##### Summary

Execute arbitrary sql

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field-'></a>
### FieldsMatch() `method`

##### Summary

Do the fields in code and database match (some implementations are case insensitive)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean-'></a>
### Insert(table,sql,updatesAutoIncrement) `method`

##### Summary

Insert a record

##### Returns

The value of the auto-increment record id of the newly inserted record

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL INSERT statement |
| updatesAutoIncrement | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if the insert may update an auto-increment field |

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Query-System-String-'></a>
### Query() `method`

##### Summary

Query the database, and return JObjects for each record returned

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-QueryOne-System-String-'></a>
### QueryOne() `method`

##### Summary

Query the database, and return the first record matching the query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote any kind of data for inclusion in a SQL query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Rollback'></a>
### Rollback() `method`

##### Summary

Rollback transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-Tables'></a>
### Tables() `method`

##### Summary

Get a Dictionary of existing tables in the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SQLiteDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}-'></a>
### UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex) `method`

##### Summary

Upgrade the table definition

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| code | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Defintiion required, from code |
| database | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Definition in database |
| insert | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to insert |
| update | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to change |
| remove | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to remove |
| insertFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to insert |
| dropFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to remove |
| insertIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to insert |
| dropIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to remove |

<a name='M-CodeFirstWebFramework-SQLiteDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View-'></a>
### ViewsMatch() `method`

##### Summary

Do the views in code and database match

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-SQLiteDateDiff'></a>
## SQLiteDateDiff `type`

##### Namespace

CodeFirstWebFramework

##### Summary

DATEDIFF function (like MySql's)

<a name='T-CodeFirstWebFramework-SQLiteSum'></a>
## SQLiteSum `type`

##### Namespace

CodeFirstWebFramework

##### Summary

SUM function which rounds as it sums, so it works like MySql's

<a name='T-CodeFirstWebFramework-ServerConfig'></a>
## ServerConfig `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Configuration for a web server

<a name='F-CodeFirstWebFramework-ServerConfig-AdditionalAssemblies'></a>
### AdditionalAssemblies `constants`

##### Summary

Additional Assemblies to load to provide the required functionality

<a name='F-CodeFirstWebFramework-ServerConfig-ConnectionString'></a>
### ConnectionString `constants`

##### Summary

Database connection string

<a name='F-CodeFirstWebFramework-ServerConfig-CookieTimeoutMinutes'></a>
### CookieTimeoutMinutes `constants`

##### Summary

Cookie timeout in minutes

<a name='F-CodeFirstWebFramework-ServerConfig-Database'></a>
### Database `constants`

##### Summary

Database type

<a name='F-CodeFirstWebFramework-ServerConfig-Email'></a>
### Email `constants`

##### Summary

Email address from which to send emails

<a name='F-CodeFirstWebFramework-ServerConfig-Namespace'></a>
### Namespace `constants`

##### Summary

Namespace in which to look for AppModules

<a name='F-CodeFirstWebFramework-ServerConfig-NamespaceDef'></a>
### NamespaceDef `constants`

##### Summary

Details of the namespace

<a name='F-CodeFirstWebFramework-ServerConfig-Port'></a>
### Port `constants`

##### Summary

The port the web server listens on

<a name='F-CodeFirstWebFramework-ServerConfig-ServerAlias'></a>
### ServerAlias `constants`

##### Summary

Other names allowed, separated by spaces

<a name='F-CodeFirstWebFramework-ServerConfig-ServerName'></a>
### ServerName `constants`

##### Summary

Name part of the url

<a name='F-CodeFirstWebFramework-ServerConfig-Title'></a>
### Title `constants`

##### Summary

Title for web pages

<a name='M-CodeFirstWebFramework-ServerConfig-Matches-System-Uri-'></a>
### Matches() `method`

##### Summary

Whether this server serves for the host name

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-WebServer-Session'></a>
## Session `type`

##### Namespace

CodeFirstWebFramework.WebServer

##### Summary

Simple session

<a name='M-CodeFirstWebFramework-WebServer-Session-#ctor-CodeFirstWebFramework-WebServer-'></a>
### #ctor(server) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| server | [CodeFirstWebFramework.WebServer](#T-CodeFirstWebFramework-WebServer 'CodeFirstWebFramework.WebServer') |  |

<a name='F-CodeFirstWebFramework-WebServer-Session-Expires'></a>
### Expires `constants`

##### Summary

When the session expires

<a name='F-CodeFirstWebFramework-WebServer-Session-Server'></a>
### Server `constants`

##### Summary

The WebServer owning the session (or null)

<a name='F-CodeFirstWebFramework-WebServer-Session-User'></a>
### User `constants`

##### Summary

Logged in user (or null if none)

<a name='P-CodeFirstWebFramework-WebServer-Session-Cookie'></a>
### Cookie `property`

##### Summary

The session cookie

<a name='P-CodeFirstWebFramework-WebServer-Session-Object'></a>
### Object `property`

##### Summary

Arbitrary JObject stored in session for later access

<a name='M-CodeFirstWebFramework-WebServer-Session-Dispose'></a>
### Dispose() `method`

##### Summary

Free any resources

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Settings'></a>
## Settings `type`

##### Namespace

CodeFirstWebFramework

##### Summary

The Settings record from the Settings table

<a name='F-CodeFirstWebFramework-Settings-DbVersion'></a>
### DbVersion `constants`

##### Summary

Database version

<a name='F-CodeFirstWebFramework-Settings-Skin'></a>
### Skin `constants`

##### Summary

Display skin to use

<a name='F-CodeFirstWebFramework-Settings-idSettings'></a>
### idSettings `constants`

##### Summary

Record id

<a name='P-CodeFirstWebFramework-Settings-AppVersion'></a>
### AppVersion `property`

##### Summary

The application version

<a name='P-CodeFirstWebFramework-Settings-Id'></a>
### Id `property`

##### Summary

Record id

<a name='T-CodeFirstWebFramework-SqlServerDatabase'></a>
## SqlServerDatabase `type`

##### Namespace

CodeFirstWebFramework

##### Summary

DbInterface for Sql Server

<a name='M-CodeFirstWebFramework-SqlServerDatabase-#ctor-CodeFirstWebFramework-Database,System-String-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-BeginTransaction'></a>
### BeginTransaction() `method`

##### Summary

Begin transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Cast-System-String,System-String-'></a>
### Cast() `method`

##### Summary

Return SQL to cast a value to a type

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-CleanDatabase'></a>
### CleanDatabase() `method`

##### Summary

Clean up database (does nothing here)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Commit'></a>
### Commit() `method`

##### Summary

Commit transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-CreateIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### CreateIndex() `method`

##### Summary

Create an index from a table and index definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-CreateTable-CodeFirstWebFramework-Table-'></a>
### CreateTable() `method`

##### Summary

Create a table from a Table definition

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Dispose'></a>
### Dispose() `method`

##### Summary

Roll back any uncommitted transaction and close the connection

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-DropIndex-CodeFirstWebFramework-Table,CodeFirstWebFramework-Index-'></a>
### DropIndex() `method`

##### Summary

Drop an index

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-DropTable-CodeFirstWebFramework-Table-'></a>
### DropTable() `method`

##### Summary

Drop a table

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Execute-System-String-'></a>
### Execute() `method`

##### Summary

Execute arbitrary sql

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-FieldsMatch-CodeFirstWebFramework-Table,CodeFirstWebFramework-Field,CodeFirstWebFramework-Field-'></a>
### FieldsMatch() `method`

##### Summary

Determine whether two fields are the same

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Insert-CodeFirstWebFramework-Table,System-String,System-Boolean-'></a>
### Insert(table,sql,updatesAutoIncrement) `method`

##### Summary

Insert a record

##### Returns

The value of the auto-increment record id of the newly inserted record

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| table | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL INSERT statement |
| updatesAutoIncrement | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | True if the insert may update an auto-increment field |

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Query-System-String-'></a>
### Query() `method`

##### Summary

Query the database, and return JObjects for each record returned

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-QueryOne-System-String-'></a>
### QueryOne() `method`

##### Summary

Query the database, and return the first record matching the query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Quote-System-Object-'></a>
### Quote() `method`

##### Summary

Quote any kind of data for inclusion in a SQL query

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Rollback'></a>
### Rollback() `method`

##### Summary

Rollback transaction

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-Tables'></a>
### Tables() `method`

##### Summary

Get a Dictionary of existing tables in the database

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-SqlServerDatabase-UpgradeTable-CodeFirstWebFramework-Table,CodeFirstWebFramework-Table,System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Field},System-Collections-Generic-List{CodeFirstWebFramework-Index},System-Collections-Generic-List{CodeFirstWebFramework-Index}-'></a>
### UpgradeTable(code,database,insert,update,remove,insertFK,dropFK,insertIndex,dropIndex) `method`

##### Summary

Upgrade the table definition

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| code | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Defintiion required, from code |
| database | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Definition in database |
| insert | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to insert |
| update | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to change |
| remove | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Fields to remove |
| insertFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to insert |
| dropFK | [System.Collections.Generic.List{CodeFirstWebFramework.Field}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Field}') | Foreign keys to remove |
| insertIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to insert |
| dropIndex | [System.Collections.Generic.List{CodeFirstWebFramework.Index}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.List 'System.Collections.Generic.List{CodeFirstWebFramework.Index}') | Indexes to remove |

<a name='M-CodeFirstWebFramework-SqlServerDatabase-ViewsMatch-CodeFirstWebFramework-View,CodeFirstWebFramework-View-'></a>
### ViewsMatch() `method`

##### Summary

Do the views in code and database match

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Table'></a>
## Table `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Table definition

<a name='M-CodeFirstWebFramework-Table-#ctor-System-String,CodeFirstWebFramework-Field[],CodeFirstWebFramework-Index[]-'></a>
### #ctor() `constructor`

##### Summary

Constructor

##### Parameters

This constructor has no parameters.

<a name='M-CodeFirstWebFramework-Table-#ctor-System-Type-'></a>
### #ctor() `constructor`

##### Summary

For TableForOrDefault - just provides JObject type conversion

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-Table-Fields'></a>
### Fields `constants`

##### Summary

The fields in the table

<a name='F-CodeFirstWebFramework-Table-Type'></a>
### Type `constants`

##### Summary

The C# type to which this table relates

<a name='P-CodeFirstWebFramework-Table-Indexes'></a>
### Indexes `property`

##### Summary

The indexes

<a name='P-CodeFirstWebFramework-Table-IsView'></a>
### IsView `property`

##### Summary

Whether this is a View rather than a native table

<a name='P-CodeFirstWebFramework-Table-Name'></a>
### Name `property`

##### Summary

Table name

<a name='P-CodeFirstWebFramework-Table-PrimaryKey'></a>
### PrimaryKey `property`

##### Summary

Primary key (first index)

<a name='P-CodeFirstWebFramework-Table-UpdateTable'></a>
### UpdateTable `property`

##### Summary

Table to update if update is called on a View

<a name='M-CodeFirstWebFramework-Table-FieldFor-System-String-'></a>
### FieldFor() `method`

##### Summary

Find a field by name (null if not found)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Table-ForeignKeyFieldFor-CodeFirstWebFramework-Table-'></a>
### ForeignKeyFieldFor() `method`

##### Summary

Find foreign key in this table that refers to target table (null if not found)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Table-FromJson``1-Newtonsoft-Json-Linq-JObject-'></a>
### FromJson\`\`1() `method`

##### Summary

Convert JObject to type T
If the table type is a subclass of T, return the table type cast to T

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Table-IndexFor-Newtonsoft-Json-Linq-JObject-'></a>
### IndexFor() `method`

##### Summary

Find the first index for which there are values for all fields in data (null if none)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Table-ToString'></a>
### ToString() `method`

##### Summary

For debugging

##### Returns



##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-TableAttribute'></a>
## TableAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a C# class as a database table

<a name='T-CodeFirstWebFramework-TableList'></a>
## TableList `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Sorted list of tables, such that tables referred to in a foreign key come before the referring table,
and tables referred to in a view come before the view

<a name='M-CodeFirstWebFramework-TableList-#ctor-System-Collections-Generic-IEnumerable{CodeFirstWebFramework-Table}-'></a>
### #ctor(allTables) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| allTables | [System.Collections.Generic.IEnumerable{CodeFirstWebFramework.Table}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{CodeFirstWebFramework.Table}') | The tables to add to the list |

<a name='T-CodeFirstWebFramework-TemplateSectionAttribute'></a>
## TemplateSectionAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Attribute to indicate a string field in an AppModule is to be filled from an XML element
of the same name (but in lower case) in a template (if such an element exists) when
the template is substituted in default.tmpl

<a name='T-CodeFirstWebFramework-Database-Timer'></a>
## Timer `type`

##### Namespace

CodeFirstWebFramework.Database

##### Summary

Class to time queries, and log if they exceed MaxTime (default Config.Default.SlowQuery)

<a name='M-CodeFirstWebFramework-Database-Timer-#ctor-System-String-'></a>
### #ctor(message) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') |  |

<a name='F-CodeFirstWebFramework-Database-Timer-MaxTime'></a>
### MaxTime `constants`

##### Summary

Max time (default Config.Default.SlowQuery)

<a name='M-CodeFirstWebFramework-Database-Timer-Dispose'></a>
### Dispose() `method`

##### Summary

Check elapsed time and log if it exceeds MaxTime

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-UniqueAttribute'></a>
## UniqueAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a field as part of a unique index

<a name='M-CodeFirstWebFramework-UniqueAttribute-#ctor-System-String-'></a>
### #ctor(name) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Index name |

<a name='M-CodeFirstWebFramework-UniqueAttribute-#ctor-System-String,System-Int32-'></a>
### #ctor(name,sequence) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | Index name |
| sequence | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | Sequence when multiple fields make up the index |

<a name='P-CodeFirstWebFramework-UniqueAttribute-Name'></a>
### Name `property`

##### Summary

Index name

<a name='P-CodeFirstWebFramework-UniqueAttribute-Sequence'></a>
### Sequence `property`

##### Summary

Sequence when multiple fields make up the index

<a name='T-CodeFirstWebFramework-UploadedFile'></a>
## UploadedFile `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Class to hold details of an uploaded file (from an <input type="file" />)

<a name='M-CodeFirstWebFramework-UploadedFile-#ctor-System-String,System-String-'></a>
### #ctor(name,content) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | field name |
| content | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | file data |

<a name='P-CodeFirstWebFramework-UploadedFile-Content'></a>
### Content `property`

##### Summary

File contents - Windows1252 was used to read it in, so saving it as Windows1252 will be an exact binary copy

<a name='P-CodeFirstWebFramework-UploadedFile-Name'></a>
### Name `property`

##### Summary

Field name

<a name='M-CodeFirstWebFramework-UploadedFile-Stream'></a>
### Stream() `method`

##### Summary

The file contents as a stream

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-User'></a>
## User `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Login user with permissions

<a name='F-CodeFirstWebFramework-User-AccessLevel'></a>
### AccessLevel `constants`

##### Summary

See AccessLevel class for possible values

<a name='F-CodeFirstWebFramework-User-Email'></a>
### Email `constants`

##### Summary

Email -c an be used instead of login name to login.

<a name='F-CodeFirstWebFramework-User-Login'></a>
### Login `constants`

##### Summary

Login name.

<a name='F-CodeFirstWebFramework-User-ModulePermissions'></a>
### ModulePermissions `constants`

##### Summary

True if user has different permissions for different modules/methods.

<a name='F-CodeFirstWebFramework-User-Password'></a>
### Password `constants`

##### Summary

Password.

<a name='F-CodeFirstWebFramework-User-idUser'></a>
### idUser `constants`

##### Summary

Unique id.

<a name='M-CodeFirstWebFramework-User-HashPassword-System-String-'></a>
### HashPassword() `method`

##### Summary

Hash a password using SHA and Base64

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-User-PasswordValid-System-String-'></a>
### PasswordValid() `method`

##### Summary

Check supplied password is valid

##### Returns

null if valid, or error message explaining why if not

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-Utils'></a>
## Utils `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Utility functions

<a name='F-CodeFirstWebFramework-Utils-DecimalRegex'></a>
### DecimalRegex `constants`

##### Summary

Regex to match decimals

<a name='F-CodeFirstWebFramework-Utils-IntegerRegex'></a>
### IntegerRegex `constants`

##### Summary

Regex matches integers

<a name='F-CodeFirstWebFramework-Utils-InvoiceNumber'></a>
### InvoiceNumber `constants`

##### Summary

Regex matches positive integers

<a name='F-CodeFirstWebFramework-Utils-_converter'></a>
### _converter `constants`

##### Summary

For converting between json string and JObject

<a name='F-CodeFirstWebFramework-Utils-_timeOffset'></a>
### _timeOffset `constants`

##### Summary

For testing - set this to an offset, and all dates & times will be offset by this amount.
Enables a test to be run as if the computer time clock was offset by this amount - 
i.e. the date & time were set exactly the same as when the test was first run.

<a name='F-CodeFirstWebFramework-Utils-_tz'></a>
### _tz `constants`

##### Summary

Time Zone to use throughout

<a name='P-CodeFirstWebFramework-Utils-Now'></a>
### Now `property`

##### Summary

Time Now.
Can be adjusted for test runs using _timeOffset

<a name='P-CodeFirstWebFramework-Utils-Today'></a>
### Today `property`

##### Summary

Date Today.
Can be adjusted for test runs using _timeOffset

<a name='M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,System-Collections-Specialized-NameValueCollection-'></a>
### AddRange() `method`

##### Summary

Add a NameValue collection to a JObject. Chainable.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,Newtonsoft-Json-Linq-JObject-'></a>
### AddRange() `method`

##### Summary

Add the properties of one JObject to another. Chainable.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AddRange-Newtonsoft-Json-Linq-JObject,System-Object[]-'></a>
### AddRange(self,content) `method`

##### Summary

Helper to add a list of stuff to a JObject. Chainable.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| self | [Newtonsoft.Json.Linq.JObject](#T-Newtonsoft-Json-Linq-JObject 'Newtonsoft.Json.Linq.JObject') | The Jobject to add the stuff to |
| content | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | Of the form: string, object - string = value
or JObject - adds properties of JObject
or NameValueCollection - adds collection members |

<a name='M-CodeFirstWebFramework-Utils-AsBool-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsBool() `method`

##### Summary

this[name] as a bool

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsDate-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsDate() `method`

##### Summary

this[name] as a DateTime

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsDecimal-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsDecimal() `method`

##### Summary

this[name] as a decimal

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsDouble-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsDouble() `method`

##### Summary

this[name] as a double

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsInt-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsInt() `method`

##### Summary

this[name] as an int

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsJObject-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsJObject() `method`

##### Summary

this[name] as a JObject

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-AsString-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### AsString() `method`

##### Summary

this[name] as a string

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-As``1-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### As\`\`1() `method`

##### Summary

this[name] as an arbitrary type T

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-Capitalise-System-String-'></a>
### Capitalise() `method`

##### Summary

Return this string, with first letter upper case

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-Check-System-Boolean,System-String-'></a>
### Check() `method`

##### Summary

Assert condition is true, throw a CheckException if not.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-Check-System-Boolean,System-String,System-Object[]-'></a>
### Check() `method`

##### Summary

Assert condition is true, throw a CheckException if not.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-CopyFrom``1-``0,System-Object-'></a>
### CopyFrom\`\`1() `method`

##### Summary

Copy all the relevant properties of the source object into this object.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-ExtractNumber-System-String-'></a>
### ExtractNumber() `method`

##### Summary

If s is a positive integer, return it, otherwise 0

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-IsAllNull-Newtonsoft-Json-Linq-JObject-'></a>
### IsAllNull() `method`

##### Summary

True if all properties of this are null (or if this itself is null)

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-IsDecimal-System-String-'></a>
### IsDecimal() `method`

##### Summary

Determine if a string is a valid decimal number

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-IsInteger-System-String-'></a>
### IsInteger() `method`

##### Summary

Determine if a string is a valid integer number

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-IsMissingOrNull-Newtonsoft-Json-Linq-JObject,System-String-'></a>
### IsMissingOrNull() `method`

##### Summary

True if the specified property is missing or null

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-JsonTo-System-String,System-Type-'></a>
### JsonTo() `method`

##### Summary

Convert this json string to a C# object of type t.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-JsonTo``1-System-String-'></a>
### JsonTo\`\`1() `method`

##### Summary

Convert this json string to a C# object of type T.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-Name-System-Reflection-Assembly-'></a>
### Name() `method`

##### Summary

Actual file name part of an Assembly name

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-NextToken-System-String@,System-String[]-'></a>
### NextToken() `method`

##### Summary

Split text at the first supplied delimiter.
Return the text before the delimiter, and set text to the remainder.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-RemoveQuotes-System-String-'></a>
### RemoveQuotes() `method`

##### Summary

If s starts and ends with a double-quote ("), remove them

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-SimilarTo-System-String,System-String-'></a>
### SimilarTo() `method`

##### Summary

Compare 2 strings, and return a number between 0 & 1 indicating what proportion of the words were the same.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-To-Newtonsoft-Json-Linq-JToken,System-Type-'></a>
### To() `method`

##### Summary

Convert this JObject to a C# object of type t

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-ToJToken-System-Object-'></a>
### ToJToken() `method`

##### Summary

Convert C# object to JToken

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-ToJson-System-Object-'></a>
### ToJson() `method`

##### Summary

Convert a C# object to json

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-To``1-Newtonsoft-Json-Linq-JToken-'></a>
### To\`\`1() `method`

##### Summary

Convert this JObject to a C# object of type T

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-UnCamel-System-Object-'></a>
### UnCamel() `method`

##### Summary

Convert a CamelCase variable name to human-readable form - e.g. "Camel  Case".
Accepts an object, so you can use it directly on Enum values.

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-Utils-UnCamel-System-String-'></a>
### UnCamel() `method`

##### Summary

Convert a CamelCase variable name to human-readable form - e.g. "Camel  Case"

##### Parameters

This method has no parameters.

<a name='T-CodeFirstWebFramework-View'></a>
## View `type`

##### Namespace

CodeFirstWebFramework

##### Summary

View definition

<a name='M-CodeFirstWebFramework-View-#ctor-System-String,CodeFirstWebFramework-Field[],CodeFirstWebFramework-Index[],System-String,CodeFirstWebFramework-Table-'></a>
### #ctor(name,fields,indexes,sql,updateTable) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | View name |
| fields | [CodeFirstWebFramework.Field[]](#T-CodeFirstWebFramework-Field[] 'CodeFirstWebFramework.Field[]') | Fields |
| indexes | [CodeFirstWebFramework.Index[]](#T-CodeFirstWebFramework-Index[] 'CodeFirstWebFramework.Index[]') | Indexes |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL to generate data |
| updateTable | [CodeFirstWebFramework.Table](#T-CodeFirstWebFramework-Table 'CodeFirstWebFramework.Table') | Table to update if update called on a view record |

<a name='P-CodeFirstWebFramework-View-IsView'></a>
### IsView `property`

##### Summary

Whether this is a view (always true)

<a name='P-CodeFirstWebFramework-View-Sql'></a>
### Sql `property`

##### Summary

SQL to generate data

<a name='P-CodeFirstWebFramework-View-UpdateTable'></a>
### UpdateTable `property`

##### Summary

Table to update if update called on a record from the view

<a name='T-CodeFirstWebFramework-ViewAttribute'></a>
## ViewAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Mark a C# class as a database view

<a name='M-CodeFirstWebFramework-ViewAttribute-#ctor-System-String-'></a>
### #ctor(sql) `constructor`

##### Summary

Constructor

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| sql | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | SQL to generate the view data |

<a name='F-CodeFirstWebFramework-ViewAttribute-Sql'></a>
### Sql `constants`

##### Summary

SWL to generate the view data

<a name='T-CodeFirstWebFramework-WebServer'></a>
## WebServer `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Web Server - listens for connections, and services them

<a name='M-CodeFirstWebFramework-WebServer-#ctor'></a>
### #ctor() `constructor`

##### Summary

Constructor.
You must call Config.Load before calling this.
Sets up all servers specified in the config file, loading any additional assemblies required.
Upgrades all databases to match the latest code.

##### Parameters

This constructor has no parameters.

<a name='F-CodeFirstWebFramework-WebServer-AppVersion'></a>
### AppVersion `constants`

##### Summary

The version number from the application.

<a name='F-CodeFirstWebFramework-WebServer-VersionSuffix'></a>
### VersionSuffix `constants`

##### Summary

Version suffix for including in url's to defeat long-term caching of (e.g.) javascript and css files

<a name='P-CodeFirstWebFramework-WebServer-Sessions'></a>
### Sessions `property`

##### Summary

All Active Sessions

<a name='M-CodeFirstWebFramework-WebServer-ProcessRequest-System-Object-'></a>
### ProcessRequest(listenerContext) `method`

##### Summary

Process a single request

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| listenerContext | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') |  |

<a name='M-CodeFirstWebFramework-WebServer-Start'></a>
### Start() `method`

##### Summary

Start WebServer listening for connections

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-WebServer-Stop'></a>
### Stop() `method`

##### Summary

Stop the server

##### Parameters

This method has no parameters.

<a name='M-CodeFirstWebFramework-WebServer-registerServer-System-Collections-Generic-HashSet{System-String},CodeFirstWebFramework-ServerConfig-'></a>
### registerServer(databases,server) `method`

##### Summary

Add namespace to modules list, and upgrade database, if not done already.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| databases | [System.Collections.Generic.HashSet{System.String}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.HashSet 'System.Collections.Generic.HashSet{System.String}') | HashSet of databases already upgraded |
| server | [CodeFirstWebFramework.ServerConfig](#T-CodeFirstWebFramework-ServerConfig 'CodeFirstWebFramework.ServerConfig') | ServerConfig to register |

<a name='T-CodeFirstWebFramework-WriteableAttribute'></a>
## WriteableAttribute `type`

##### Namespace

CodeFirstWebFramework

##### Summary

Indicate a field or class is writeable by default, even if it is not part of a Table
