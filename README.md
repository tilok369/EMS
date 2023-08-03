# EMS
## Employee Management System

Simple Employee management system using WPF .NET 6 framework. It uses MVVM pattern and dependency injection. 
https://gorest.co.in/ used as a REST API data source. Unit testing added with NUnit and Moq. 

## Project structure
* ***EMS.App***: Main WPF form application
* ***EMS.Service***: REST API communication service
* ***EMS.Model***: Resides data passing models between view and service.
* ***EMS.Test***: Unit test project.


## External Packages/Libaries used:
* LoadingSpinner.WPF
* Microsoft.Extensions.Hosting
* Microsoft.Extensions.DependencyInjection

## Available Functionalities
* Employee list showing
* Employee search by name and email
* Employee creation
* Employee modification
* Employee deletion
