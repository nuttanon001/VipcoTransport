using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
namespace VipcoTransport.Services.Interfaces
{
    public interface IEmployeeRepository
    {
        //TblEmployee
        IQueryable<TblEmployee> GetTblEmployees { get; }
        DataViewModel<EmployeeViewModel> GetEmployeeViewModelWithLazyLoad(LazyViewModel lazyData);
        TblEmployee GetTblEmployeeWithKey(int EmployeeID);
        TblEmployee InsertTblEmployeeToDataBase(TblEmployee nEmp);
        TblEmployee UpdateTblEmployeeToDataBase(int EmployeeID, TblEmployee uEmployee);

        //TblDriver
        IQueryable<TblDriver> GetTblDrivers { get; }
        DataViewModel<TblDriver> GetTblDriverWithFilter(string filter);
        TblDriver GetTblDriverWithKey(int DriverID);
        TblDriver InsertTblDriverToDataBase(TblDriver nDriver);
        TblDriver UpdateTblDriverToDataBase(int DriverID, TblDriver uDriver);

        //TblLocation
        IQueryable<TblLocation> GetTblLocations { get; }
        DataViewModel<TblLocation> GetTblLocationWithFilter(string filter);
        TblLocation GetTblLocationWithKey(int LocationID);
        TblLocation InsertTblLocationToDataBase(TblLocation nLocation);
        TblLocation UpdateTblLocationToDataBase(int LocationID, TblLocation uLocation);

        //TblContact
        IQueryable<TblContact> GetTblContacts { get; }
        DataViewModel<TblContact> GetTblContactWithFilter(string filter);
        TblContact GetTblContactWithKey(int ContactID);
        TblContact InsertTblContactOnlyLocationToDataBase(TblLocation nContactOnlyLocation);
        TblContact InsertTblContactToDataBase(TblContact nContact);
        TblContact UpdateTblContactToDataBase(int ContactID, TblContact uContact);

        //TblUser
        IQueryable<TblUser> GetTblUsers { get; }
        DataViewModel<TblUser> GetTblUserWithFilter(string filter);
        TblUser GetTblUserWithKey(int UserID);
        TblUser InsertTblUserToDataBase(TblUser nUser);
        TblUser UpdateTblUserToDataBase(int UserID, TblUser uUser);
        TblUser GetTblUserByUserName(string username);
        bool CheckUserProfile(string username, string password);

        // VEmployee
        IQueryable<VEmployee> GetVEmployees { get; }
        VEmployee GetVEmployeeWithKey(string EmpCode);
        string GetVEmployeeByUserName(string userName);

    }
}
