using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using VipcoTransport.Models;
using VipcoTransport.ViewModels;
using VipcoTransport.Services.Interfaces;

namespace VipcoTransport.Services.Classes
{
    public class EmployeeRepository:IEmployeeRepository
    {
        #region PrivateMembers

        ApplicationContext Context;

        private DateTime DateOfServer
        {
            get
            {
                var connection = this.Context.Database.GetDbConnection();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT SYSDATETIME()";
                connection.Open();
                var servertime = (DateTime)command.ExecuteScalar();
                connection.Close();

                return servertime;
            }
        }
        #endregion

        #region Constructor

        public EmployeeRepository(ApplicationContext ctx)
        {
            this.Context = ctx;
        }

        #endregion

        #region TblEmployee
        public IQueryable<TblEmployee> GetTblEmployees => this.Context.TblEmployee.AsNoTracking().OrderBy(x => x.EmpCode);
        public DataViewModel<EmployeeViewModel> GetEmployeeViewModelWithLazyLoad(LazyViewModel lazyData)
        {
            try
            {
                var Query = this.GetTblEmployees;

                if (!string.IsNullOrEmpty(lazyData.filter))
                {
                    foreach (var keyword in lazyData.filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.EmpCode.ToLower().Contains(keyword) ||
                                                 x.NameEng.ToLower().Contains(keyword) ||
                                                 x.NameThai.ToLower().Contains(keyword));
                    }
                }

                switch (lazyData.sortField)
                {
                    case "NameThai":
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.NameThai);
                        else
                            Query = Query.OrderBy(x => x.NameThai);
                        break;

                    default:
                        if (lazyData.sortOrder == -1)
                            Query = Query.OrderByDescending(x => x.EmpCode);
                        else
                            Query = Query.OrderBy(x => x.EmpCode);
                        break;
                }

                var totalRow = Query.Count();
                // select with rownumber
                Query = Query.Skip(lazyData.first ?? 0).Take(lazyData.rows ?? 50);

                var hasData = Query.AsEnumerable<TblEmployee>().Select((x, i) => new EmployeeViewModel
                {
                    RowNumber = (i + 1) + lazyData.first ?? 0,
                    EmpCode = x.EmpCode,
                    NameThai = x.NameThai,
                    EmployeeId = x.EmployeeId,
                });

                return new DataViewModel<EmployeeViewModel>(hasData, totalRow);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }

        public TblEmployee GetTblEmployeeWithKey(int EmployeeID)
        {
            try
            {
                if (EmployeeID > 0)
                {
                    return this.GetTblEmployees.Where(x => x.EmployeeId == EmployeeID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblEmployee InsertTblEmployeeToDataBase(TblEmployee nEmp)
        {
            try
            {
                if (nEmp != null)
                {
                    this.Context.TblEmployee.Add(nEmp);
                    return this.Context.SaveChanges() > 0 ? this.GetTblEmployeeWithKey(nEmp.EmployeeId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblEmployee UpdateTblEmployeeToDataBase(int EmployeeID, TblEmployee uEmployee)
        {
            try
            {
                var dbEmployee = this.Context.TblEmployee.Find(EmployeeID);
                if (dbEmployee != null)
                {
                    this.Context.Entry(dbEmployee).CurrentValues.SetValues(uEmployee);
                    return this.Context.SaveChanges() > 0 ? this.GetTblEmployeeWithKey(EmployeeID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }

        #endregion

        #region TblDriver
        public IQueryable<TblDriver> GetTblDrivers => this.Context.TblDriver.AsNoTracking()
                                                                  .Include(x => x.EmployeeDriveCodeNavigation)
                                                                  .Include(x => x.Car.CarBrandNavigation)
                                                                  .Include(x => x.Trailer.TrailerBrandNavigation)
                                                                  .OrderBy(x => x.EmployeeDriveCode);
        public DataViewModel<TblDriver> GetTblDriverWithFilter(string filter)
        {
            try
            {
                var Query = this.GetTblDrivers;
                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.EmployeeDriveCode.ToLower().Contains(keyword) ||
                                                    x.EmployeeDriveCodeNavigation.NameEng.ToLower().Contains(keyword) ||
                                                    x.EmployeeDriveCodeNavigation.NameThai.ToLower().Contains(keyword) ||
                                                    x.PhoneNumber.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblDriver>(Query.AsNoTracking().AsEnumerable<TblDriver>(), Query.Count());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblDriver GetTblDriverWithKey(int DriverID)
        {
            try
            {
                if (DriverID > 0)
                {
                    return this.GetTblDrivers.Where(x => x.EmployeeDriveId == DriverID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblDriver InsertTblDriverToDataBase(TblDriver nDriver)
        {
            try
            {
                if (nDriver != null)
                {
                    nDriver.CreateDate = this.DateOfServer;
                    nDriver.Creator = nDriver.Creator ?? "someone";

                    if (!string.IsNullOrEmpty(nDriver.EmployeeDriveCode))
                        nDriver.EmployeeDriveCode = this.Context.VEmployee.Find(nDriver.EmployeeDriveCode)?.EmpCode ?? "";

                    this.Context.TblDriver.Add(nDriver);
                    return this.Context.SaveChanges() > 0 ? this.GetTblDriverWithKey(nDriver.EmployeeDriveId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblDriver UpdateTblDriverToDataBase(int DriverID, TblDriver uDriver)
        {
            try
            {
                var dbDriver = this.Context.TblDriver.Find(DriverID);
                if (dbDriver != null)
                {
                    uDriver.CreateDate = dbDriver.CreateDate;
                    uDriver.ModifyDate = this.DateOfServer;
                    uDriver.Modifyer = uDriver.Modifyer ?? "someone";

                    if (uDriver.CarId == 0)
                        uDriver.CarId = null;

                    if (uDriver.TrailerId == 0)
                        uDriver.TrailerId = null;

                    if (!string.IsNullOrEmpty(uDriver.EmployeeDriveCode))
                        uDriver.EmployeeDriveCode = this.Context.VEmployee.Find(uDriver.EmployeeDriveCode)?.EmpCode ?? "";


                    this.Context.Entry(dbDriver).CurrentValues.SetValues(uDriver);
                    return this.Context.SaveChanges() > 0 ? this.GetTblDriverWithKey(DriverID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        #endregion

        #region TblLocation
        public IQueryable<TblLocation> GetTblLocations => this.Context.TblLocation
                                                                        .AsNoTracking()
                                                                        .OrderBy(x => x.LocationCode)
                                                                        .ThenBy(x => x.LocationName);
        public DataViewModel<TblLocation> GetTblLocationWithFilter(string filter)
        {
            try
            {
                var Query = this.GetTblLocations;
                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.LocationCode.ToLower().Contains(keyword) ||
                                                    x.LocationName.ToLower().Contains(keyword) ||
                                                      x.LocationAddress.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblLocation>(Query.AsEnumerable<TblLocation>(), Query.Count());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblLocation GetTblLocationWithKey(int LocationID)
        {
            try
            {
                if (LocationID > 0)
                    return this.GetTblLocations.Where(x => x.LocationId == LocationID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblLocation InsertTblLocationToDataBase(TblLocation nLocation)
        {
            try
            {
                if (nLocation != null)
                {
                    nLocation.CreateDate = this.DateOfServer;
                    nLocation.Creator = nLocation.Creator ?? "someone";

                    this.Context.TblLocation.Add(nLocation);
                    return this.Context.SaveChanges() > 0 ? this.GetTblLocationWithKey(nLocation.LocationId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblLocation UpdateTblLocationToDataBase(int LocationID, TblLocation uLocation)
        {
            try
            {
                var dbLocation = this.Context.TblLocation.Find(LocationID);
                if (dbLocation != null)
                {
                    uLocation.CreateDate = dbLocation.CreateDate;
                    uLocation.ModifyDate = this.DateOfServer;
                    uLocation.Modifyer = uLocation.Modifyer ?? "someone";

                    this.Context.Entry(dbLocation).CurrentValues.SetValues(uLocation);
                    return this.Context.SaveChanges() > 0 ? this.GetTblLocationWithKey(LocationID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        #endregion

        #region TblContact
        public IQueryable<TblContact> GetTblContacts => this.Context.TblContact.AsNoTracking()
                                                                    .Include(x => x.LocationNavigation)
                                                                    .OrderBy(x => x.LocationNavigation.LocationCode.Length)
                                                                    .ThenBy(x => x.LocationNavigation.LocationCode);
        public DataViewModel<TblContact> GetTblContactWithFilter(string filter)
        {
            try
            {
                var Query = this.GetTblContacts;

                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.ContactName.ToLower().Contains(keyword) ||
                                                    x.ContactPhone.ToLower().Contains(keyword) ||
                                                    x.LocationNavigation.LocationCode.ToLower().Contains(keyword) ||
                                                    x.LocationNavigation.LocationName.ToLower().Contains(keyword) ||
                                                    x.LocationNavigation.LocationAddress.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblContact>(Query.AsEnumerable<TblContact>(), Query.Count());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblContact GetTblContactWithKey(int ContactID)
        {
            try
            {
                if (ContactID > 0)
                    return this.GetTblContacts.Where(x => x.ContactId == ContactID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblContact InsertTblContactOnlyLocationToDataBase(TblLocation nContactOnlyLocation)
        {
            try
            {
                if (nContactOnlyLocation != null)
                {
                    if (this.GetTblLocations.Where(x => x.LocationName.Trim().ToLower() == nContactOnlyLocation.LocationName.Trim().ToLower()).Any())
                        return null;

                    string LocationCode = "OZ" + ((this.GetTblLocations.Where(x => x.LocationCode.StartsWith("OZ"))?.Count() ?? 1) + 1).ToString("0");

                    var nContact = new TblContact()
                    {
                        ContactName = "",
                        ContactPhone = "",
                        CreateDate = this.DateOfServer,
                        Creator = nContactOnlyLocation.Creator ?? "someone",
                        LocationNavigation = new TblLocation()
                        {
                            CreateDate = this.DateOfServer,
                            Creator = nContactOnlyLocation.Creator ?? "someone",
                            LocationAddress = nContactOnlyLocation.LocationAddress,
                            LocationCode = LocationCode,
                            LocationName = nContactOnlyLocation.LocationName
                        }
                    };

                    this.Context.TblContact.Add(nContact);
                    return this.Context.SaveChanges() > 0 ? this.GetTblContactWithKey(nContact.ContactId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblContact InsertTblContactToDataBase(TblContact nContact)
        {
            try
            {
                if (nContact != null)
                {
                    nContact.CreateDate = this.DateOfServer;
                    nContact.Creator = nContact.Creator ?? "someone";

                    this.Context.TblContact.Add(nContact);
                    return this.Context.SaveChanges() > 0 ? this.GetTblContactWithKey(nContact.ContactId) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblContact UpdateTblContactToDataBase(int ContactID, TblContact uContact)
        {
            try
            {
                var dbContact = this.Context.TblContact.Find(ContactID);
                if (dbContact != null)
                {
                    uContact.CreateDate = dbContact.CreateDate;
                    uContact.ModifyDate = this.DateOfServer;
                    uContact.Modifyer = uContact.Modifyer ?? "someone";

                    this.Context.Entry(dbContact).CurrentValues.SetValues(uContact);
                    return this.Context.SaveChanges() > 0 ? this.GetTblContactWithKey(ContactID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        #endregion

        #region TblUser

        public IQueryable<TblUser> GetTblUsers => this.Context.TblUser
                                                              .Include(x => x.EmployeeCodeNavigation)
                                                              .AsNoTracking()
                                                              .OrderBy(x => x.EmployeeCode.Length)
                                                              .ThenBy(x => x.EmployeeCode);
        public DataViewModel<TblUser> GetTblUserWithFilter(string filter)
        {
            try
            {
                var Query = this.GetTblUsers;
                if (!string.IsNullOrEmpty(filter))
                {
                    foreach (var keyword in filter.Trim().ToLower().Split(null))
                    {
                        Query = Query.Where(x => x.EmployeeCode.ToLower().Contains(keyword) ||
                                                 x.EmployeeCodeNavigation.NameThai.ToLower().Contains(keyword) ||
                                                 x.EmployeeCodeNavigation.NameEng.ToLower().Contains(keyword) ||
                                                 x.EmployeeCodeNavigation.SectionName.ToLower().Contains(keyword) ||
                                                 x.MailAddress.ToLower().Contains(keyword) ||
                                                 x.Username.ToLower().Contains(keyword));
                    }
                }
                return new DataViewModel<TblUser>(Query.AsEnumerable<TblUser>(), Query.Count());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblUser GetTblUserWithKey(int UserID)
        {
            try
            {
                if (UserID > 0)
                {
                    return this.GetTblUsers.Where(x => x.UserId == UserID).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }
        public TblUser InsertTblUserToDataBase(TblUser nUser)
        {
            try
            {
                if (nUser != null)
                {
                    nUser.CreateDate = this.DateOfServer;
                    nUser.Creator = nUser.Creator ?? "someone";

                    this.Context.TblUser.Add(nUser);
                    return this.Context.SaveChanges() > 0 ? this.GetTblUserWithKey(nUser.UserId) : null;

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public TblUser UpdateTblUserToDataBase(int UserID, TblUser uUser)
        {
            try
            {
                var dbUser = this.Context.TblUser.Find(UserID);
                if (dbUser != null)
                {
                    uUser.CreateDate = dbUser.CreateDate;
                    uUser.ModifyDate = this.DateOfServer;
                    uUser.Modifyer = uUser.Modifyer ?? "someone";

                    if (string.IsNullOrEmpty(uUser.Password))
                        uUser.Password = dbUser.Password;

                    this.Context.Entry(dbUser).CurrentValues.SetValues(uUser);
                    return this.Context.SaveChanges() > 0 ? this.GetTblUserWithKey(UserID) : null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }
            return null;
        }
        public TblUser GetTblUserByUserName(string username)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    return this.Context.TblUser.Where(x => x.Username == username).Include(x => x.EmployeeCodeNavigation).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }
            return null;
        }
        public bool CheckUserProfile(string username, string password)
        {
            bool canlogin = false;
            try
            {
                var query = Context.TblUser.Where(x => x.Username.ToLower() == username.ToLower() && x.Password.ToLower() == password.ToLower());

                canlogin = query.Any();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"has error\r\n{ex.ToString()}");
            }

            return canlogin;
        }

        #endregion

        #region vEmployee

        public IQueryable<VEmployee> GetVEmployees => this.Context.VEmployee.AsNoTracking().OrderBy(x => x.EmpCode.Length).ThenBy(x => x.EmpCode);
        public VEmployee GetVEmployeeWithKey(string EmpCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmpCode))
                {
                    return this.GetVEmployees.Where(x => x.EmpCode == EmpCode).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Has error {ex.ToString()}");
            }

            return null;
        }

        public string GetVEmployeeByUserName(string userName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    return this.Context.TblUser.Where(x => x.Username == userName).FirstOrDefault().EmployeeCode;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Has error " + ex.ToString());
            }

            return null;
        }

        #endregion
    }
}
