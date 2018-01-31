using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace VipcoTransport.Models
{
    public partial class ApplicationContext : DbContext
    {
        public virtual DbSet<TblAttachFile> TblAttachFile { get; set; }
        public virtual DbSet<TblCarBrand> TblCarBrand { get; set; }
        public virtual DbSet<TblCarData> TblCarData { get; set; }
        public virtual DbSet<TblCarHasCompany> TblCarHasCompany { get; set; }
        public virtual DbSet<TblCarType> TblCarType { get; set; }
        public virtual DbSet<TblCompany> TblCompany { get; set; }
        public virtual DbSet<TblContact> TblContact { get; set; }
        public virtual DbSet<TblDriver> TblDriver { get; set; }
        public virtual DbSet<TblDriverHasCompany> TblDriverHasCompany { get; set; }
        public virtual DbSet<TblEmployee> TblEmployee { get; set; }
        public virtual DbSet<TblLocation> TblLocation { get; set; }
        public virtual DbSet<TblReqHasCompany> TblReqHasCompany { get; set; }
        public virtual DbSet<TblRequestHasDataTransport> TblRequestHasDataTransport { get; set; }
        public virtual DbSet<TblTrailerTruck> TblTrailerTruck { get; set; }
        public virtual DbSet<TblTranHasCompany> TblTranHasCompany { get; set; }
        public virtual DbSet<TblTransportData> TblTransportData { get; set; }
        public virtual DbSet<TblTransportRequestHasAttach> TblTransportRequestHasAttach { get; set; }
        public virtual DbSet<TblTransportRequestion> TblTransportRequestion { get; set; }
        public virtual DbSet<TblTransportType> TblTransportType { get; set; }
        public virtual DbSet<TblTruckHasCompany> TblTruckHasCompany { get; set; }
        public virtual DbSet<TblUser> TblUser { get; set; }
        public virtual DbSet<TblUserHasCompany> TblUserHasCompany { get; set; }
        public virtual DbSet<VEmployee> VEmployee { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblAttachFile>(entity =>
            {
                entity.HasKey(e => e.AttachId)
                    .HasName("PK_AttachFile");

                entity.ToTable("tblAttachFile");

                entity.Property(e => e.AttachId).HasColumnName("AttachID");

                entity.Property(e => e.AttachAddress)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.AttachFileName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Modifyer).HasMaxLength(25);
            });

            modelBuilder.Entity<TblCarBrand>(entity =>
            {
                entity.HasKey(e => e.BrandId)
                    .HasName("PK_tblVehicleBrand");

                entity.ToTable("tblCarBrand");

                entity.HasIndex(e => e.BrandName)
                    .HasName("IX_tblCarBrand")
                    .IsUnique();

                entity.Property(e => e.BrandId).HasColumnName("BrandID");

                entity.Property(e => e.BrandDesc).HasMaxLength(200);

                entity.Property(e => e.BrandName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);
            });

            modelBuilder.Entity<TblCarData>(entity =>
            {
                entity.HasKey(e => e.CarId)
                    .HasName("PK_tblCarData");

                entity.ToTable("tblCarData");

                entity.HasIndex(e => e.CarNo)
                    .HasName("IX_tblCarData")
                    .IsUnique();

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.ActDate)
                    .HasColumnName("Act_Date")
                    .HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.ActInsurance)
                    .HasColumnName("Act_Insurance")
                    .HasMaxLength(50);

                entity.Property(e => e.CarDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.CarNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Images).HasColumnType("image");

                entity.Property(e => e.InsurDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Insurance).HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.RegisterNo).HasMaxLength(20);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.Status).HasDefaultValueSql("0");

                entity.HasOne(d => d.CarBrandNavigation)
                    .WithMany(p => p.TblCarData)
                    .HasForeignKey(d => d.CarBrand)
                    .HasConstraintName("FK_tblCarData_tblCarBrand");

                entity.HasOne(d => d.CarType)
                    .WithMany(p => p.TblCarData)
                    .HasForeignKey(d => d.CarTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblCarData_tblCarType");
            });

            modelBuilder.Entity<TblCarHasCompany>(entity =>
            {
                entity.HasKey(e => e.CarHasCompanyId)
                    .HasName("PK_tblCarHasCompany");

                entity.ToTable("tblCarHasCompany", "company");

                entity.Property(e => e.CarHasCompanyId).HasColumnName("CarHasCompanyID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.TblCarHasCompany)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblCarHasCompany_tblCarData");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblCarHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblCarHasCompany_tblCompany");
            });

            modelBuilder.Entity<TblCarType>(entity =>
            {
                entity.HasKey(e => e.CarTypeId)
                    .HasName("PK_tblCarType_1");

                entity.ToTable("tblCarType");

                entity.HasIndex(e => e.CarTypeNo)
                    .HasName("IX_tblCarType")
                    .IsUnique();

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.CarTypeDesc).HasMaxLength(50);

                entity.Property(e => e.CarTypeNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.Remark).HasMaxLength(50);
            });

            modelBuilder.Entity<TblCompany>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK_tblCompany");

                entity.ToTable("tblCompany", "company");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Address1).HasMaxLength(200);

                entity.Property(e => e.Address2).HasMaxLength(200);

                entity.Property(e => e.CompanyCode).HasMaxLength(20);

                entity.Property(e => e.CompanyName).HasMaxLength(200);

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.Property(e => e.Telephone).HasMaxLength(50);
            });

            modelBuilder.Entity<TblContact>(entity =>
            {
                entity.HasKey(e => e.ContactId)
                    .HasName("PK_tblContact");

                entity.ToTable("tblContact");

                entity.Property(e => e.ContactId).HasColumnName("ContactID");

                entity.Property(e => e.ContactName).HasMaxLength(50);

                entity.Property(e => e.ContactPhone).HasMaxLength(20);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.HasOne(d => d.LocationNavigation)
                    .WithMany(p => p.TblContact)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblContact_tblLocation");
            });

            modelBuilder.Entity<TblDriver>(entity =>
            {
                entity.HasKey(e => e.EmployeeDriveId)
                    .HasName("PK_tblDriver");

                entity.ToTable("tblDriver");

                entity.Property(e => e.EmployeeDriveId).HasColumnName("EmployeeDriveID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.EmployeeDriveCode).HasMaxLength(20);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.TrailerId).HasColumnName("TrailerID");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.TblDriver)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK_tblDriver_tblCarData");

                entity.HasOne(d => d.EmployeeDriveCodeNavigation)
                    .WithMany(p => p.TblDriver)
                    .HasForeignKey(d => d.EmployeeDriveCode)
                    .HasConstraintName("FK_tblDriver_tblDriver");

                entity.HasOne(d => d.Trailer)
                    .WithMany(p => p.TblDriver)
                    .HasForeignKey(d => d.TrailerId)
                    .HasConstraintName("FK_tblDriver_tblTrailerTruck");
            });

            modelBuilder.Entity<TblDriverHasCompany>(entity =>
            {
                entity.HasKey(e => e.DriverHasCompanyId)
                    .HasName("PK_tblDriverHasCompany");

                entity.ToTable("tblDriverHasCompany", "company");

                entity.Property(e => e.DriverHasCompanyId).HasColumnName("DriverHasCompanyID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.EmployeeDriveId).HasColumnName("EmployeeDriveID");

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblDriverHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblDriverHasCompany_tblCompany");

                entity.HasOne(d => d.EmployeeDrive)
                    .WithMany(p => p.TblDriverHasCompany)
                    .HasForeignKey(d => d.EmployeeDriveId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblDriverHasCompany_tblDriver");
            });

            modelBuilder.Entity<TblEmployee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("PK_tblEmployee");

                entity.ToTable("tblEmployee");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.EmpCode).HasMaxLength(20);

                entity.Property(e => e.NameEng).HasMaxLength(100);

                entity.Property(e => e.NameThai).HasMaxLength(100);
            });

            modelBuilder.Entity<TblLocation>(entity =>
            {
                entity.HasKey(e => e.LocationId)
                    .HasName("PK_tblLocation");

                entity.ToTable("tblLocation");

                entity.HasIndex(e => e.LocationCode)
                    .HasName("IX_tblLocation")
                    .IsUnique();

                entity.Property(e => e.LocationId).HasColumnName("LocationID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.LocationAddress).HasMaxLength(200);

                entity.Property(e => e.LocationCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LocationName).HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);
            });

            modelBuilder.Entity<TblReqHasCompany>(entity =>
            {
                entity.HasKey(e => e.ReqHasCompanyId)
                    .HasName("PK_tblReqHasCompany");

                entity.ToTable("tblReqHasCompany", "company");

                entity.Property(e => e.ReqHasCompanyId).HasColumnName("ReqHasCompanyID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.Property(e => e.TransportRequestId).HasColumnName("TransportRequestID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblReqHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblReqHasCompany_tblCompany");

                entity.HasOne(d => d.TransportRequest)
                    .WithMany(p => p.TblReqHasCompany)
                    .HasForeignKey(d => d.TransportRequestId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblReqHasCompany_tblTransportRequestion");
            });

            modelBuilder.Entity<TblRequestHasDataTransport>(entity =>
            {
                entity.HasKey(e => new { e.RequestHasDataTransportId, e.TransportRequestId, e.TransportId })
                    .HasName("PK_RequestHasDataTransport");

                entity.ToTable("tblRequestHasDataTransport");

                entity.Property(e => e.RequestHasDataTransportId)
                    .HasColumnName("RequestHasDataTransportID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TransportRequestId).HasColumnName("TransportRequestID");

                entity.Property(e => e.TransportId).HasColumnName("TransportID");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.HasOne(d => d.Transport)
                    .WithMany(p => p.TblRequestHasDataTransport)
                    .HasForeignKey(d => d.TransportId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblRequestHasDataTransport_tblTransportData");

                entity.HasOne(d => d.TransportRequest)
                    .WithMany(p => p.TblRequestHasDataTransport)
                    .HasForeignKey(d => d.TransportRequestId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_tblRequestHasDataTransport_tblTransportRequestion");
            });

            modelBuilder.Entity<TblTrailerTruck>(entity =>
            {
                entity.HasKey(e => e.TrailerId)
                    .HasName("PK_tblTrailerTruck");

                entity.ToTable("tblTrailerTruck");

                entity.HasIndex(e => e.TrailerNo)
                    .HasName("IX_tblTrailerTruck")
                    .IsUnique();

                entity.Property(e => e.TrailerId).HasColumnName("TrailerID");

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Images).HasColumnType("image");

                entity.Property(e => e.InsurDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Insurance).HasMaxLength(50);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.RegisterNo).HasMaxLength(20);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.TrailerDesc).HasMaxLength(80);

                entity.Property(e => e.TrailerNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.CarType)
                    .WithMany(p => p.TblTrailerTruck)
                    .HasForeignKey(d => d.CarTypeId)
                    .HasConstraintName("FK_tblTrailerTruck_tblCarType");

                entity.HasOne(d => d.TrailerBrandNavigation)
                    .WithMany(p => p.TblTrailerTruck)
                    .HasForeignKey(d => d.TrailerBrand)
                    .HasConstraintName("FK_tblTrailerTruck_tblCarBrand");
            });

            modelBuilder.Entity<TblTranHasCompany>(entity =>
            {
                entity.HasKey(e => e.TranHasCompanyId)
                    .HasName("PK_tblTranHasCompany");

                entity.ToTable("tblTranHasCompany", "company");

                entity.Property(e => e.TranHasCompanyId).HasColumnName("TranHasCompanyID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.Property(e => e.TransportId).HasColumnName("TransportID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblTranHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblTranHasCompany_tblCompany");

                entity.HasOne(d => d.Transport)
                    .WithMany(p => p.TblTranHasCompany)
                    .HasForeignKey(d => d.TransportId)
                    .HasConstraintName("FK_tblTranHasCompany_tblTransportData");
            });

            modelBuilder.Entity<TblTransportData>(entity =>
            {
                entity.HasKey(e => e.TransportId)
                    .HasName("PK_tblTransportData");

                entity.ToTable("tblTransportData");

                entity.Property(e => e.TransportId).HasColumnName("TransportID");

                entity.Property(e => e.CarId).HasColumnName("CarID");

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.EmployeeDriveCode).HasMaxLength(20);

                entity.Property(e => e.EmployeeRequestCode).HasMaxLength(20);

                entity.Property(e => e.FinalTime).HasMaxLength(10);

                entity.Property(e => e.JobInfo).HasMaxLength(20);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.StatusNo).HasMaxLength(10);

                entity.Property(e => e.TrailerId).HasColumnName("TrailerID");

                entity.Property(e => e.TransDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.TransTime).HasMaxLength(10);

                entity.Property(e => e.TransportNo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.TransportTypeId).HasColumnName("TransportTypeID");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK_tblTransportData_tblCarData");

                entity.HasOne(d => d.CarType)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.CarTypeId)
                    .HasConstraintName("FK_tblTransportData_tblCarType");

                entity.HasOne(d => d.ContactDestinationNavigation)
                    .WithMany(p => p.TblTransportDataContactDestinationNavigation)
                    .HasForeignKey(d => d.ContactDestination)
                    .HasConstraintName("FK_tblTransportData_tblContact1");

                entity.HasOne(d => d.ContactSourceNavigation)
                    .WithMany(p => p.TblTransportDataContactSourceNavigation)
                    .HasForeignKey(d => d.ContactSource)
                    .HasConstraintName("FK_tblTransportData_tblContact");

                entity.HasOne(d => d.EmployeeDriveNavigation)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.EmployeeDrive)
                    .HasConstraintName("FK_tblTransportData_tblDriver");

                entity.HasOne(d => d.EmployeeRequestCodeNavigation)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.EmployeeRequestCode)
                    .HasConstraintName("FK_tblTransportData_V_Employee");

                entity.HasOne(d => d.Trailer)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.TrailerId)
                    .HasConstraintName("FK_tblTransportData_tblTrailerTruck");

                entity.HasOne(d => d.TransportType)
                    .WithMany(p => p.TblTransportData)
                    .HasForeignKey(d => d.TransportTypeId)
                    .HasConstraintName("FK_tblTransportData_tblTransportType");
            });

            modelBuilder.Entity<TblTransportRequestHasAttach>(entity =>
            {
                entity.HasKey(e => new { e.TransportRequestHasAttachId, e.TransportRequestId, e.AttachId })
                    .HasName("PK_TransportRequestHasAttach");

                entity.ToTable("tblTransportRequestHasAttach");

                entity.Property(e => e.TransportRequestHasAttachId)
                    .HasColumnName("TransportRequestHasAttachID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TransportRequestId).HasColumnName("TransportRequestID");

                entity.Property(e => e.AttachId).HasColumnName("AttachID");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Modifyer).HasMaxLength(25);
            });

            modelBuilder.Entity<TblTransportRequestion>(entity =>
            {
                entity.HasKey(e => e.TransportRequestId)
                    .HasName("PK_tblTransportRequestion");

                entity.ToTable("tblTransportRequestion");

                entity.Property(e => e.TransportRequestId).HasColumnName("TransportRequestID");

                entity.Property(e => e.CarTypeId).HasColumnName("CarTypeID");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.DepartmentCode).HasMaxLength(200);

                entity.Property(e => e.EmailResponse).HasMaxLength(200);

                entity.Property(e => e.EmployeeRequestCode).HasMaxLength(20);

                entity.Property(e => e.JobInfo).HasMaxLength(20);

                entity.Property(e => e.ModifyDate).HasDefaultValueSql("sysdatetime()");

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.ProblemName).HasMaxLength(50);

                entity.Property(e => e.ProblemPhone).HasMaxLength(20);

                entity.Property(e => e.TransportReqNo).HasMaxLength(50);

                entity.Property(e => e.TransportTime).HasMaxLength(10);

                entity.Property(e => e.TransportType).HasMaxLength(20);

                entity.Property(e => e.TransportTypeId).HasColumnName("TransportTypeID");

                entity.HasOne(d => d.CarType)
                    .WithMany(p => p.TblTransportRequestion)
                    .HasForeignKey(d => d.CarTypeId)
                    .HasConstraintName("FK_tblTransportRequestion_tblCarType");

                entity.HasOne(d => d.ContactDestinationNavigation)
                    .WithMany(p => p.TblTransportRequestionContactDestinationNavigation)
                    .HasForeignKey(d => d.ContactDestination)
                    .HasConstraintName("FK_tblTransportRequestion_tblContact1");

                entity.HasOne(d => d.ContactSourceNavigation)
                    .WithMany(p => p.TblTransportRequestionContactSourceNavigation)
                    .HasForeignKey(d => d.ContactSource)
                    .HasConstraintName("FK_tblTransportRequestion_tblContact");

                entity.HasOne(d => d.EmployeeRequestCodeNavigation)
                    .WithMany(p => p.TblTransportRequestion)
                    .HasForeignKey(d => d.EmployeeRequestCode)
                    .HasConstraintName("FK_tblTransportRequestion_V_Employee");

                entity.HasOne(d => d.TransportTypeNavigation)
                    .WithMany(p => p.TblTransportRequestion)
                    .HasForeignKey(d => d.TransportTypeId)
                    .HasConstraintName("FK_tblTransportRequestion_tblTransportType");
            });

            modelBuilder.Entity<TblTransportType>(entity =>
            {
                entity.HasKey(e => e.TransportTypeId)
                    .HasName("PK_tblTransportType");

                entity.ToTable("tblTransportType");

                entity.Property(e => e.TransportTypeId).HasColumnName("TransportTypeID");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.Remark).HasMaxLength(50);

                entity.Property(e => e.TransportTypeDesc).HasMaxLength(50);
            });

            modelBuilder.Entity<TblTruckHasCompany>(entity =>
            {
                entity.HasKey(e => e.TruckHasCompanyId)
                    .HasName("PK_tblTruckHasCompany");

                entity.ToTable("tblTruckHasCompany", "company");

                entity.Property(e => e.TruckHasCompanyId).HasColumnName("TruckHasCompanyID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.Property(e => e.TrailerId).HasColumnName("TrailerID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblTruckHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblTruckHasCompany_tblCompany");

                entity.HasOne(d => d.Trailer)
                    .WithMany(p => p.TblTruckHasCompany)
                    .HasForeignKey(d => d.TrailerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_tblTruckHasCompany_tblTrailerTruck");
            });

            modelBuilder.Entity<TblUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_tblUser");

                entity.ToTable("tblUser");

                entity.HasIndex(e => e.Username)
                    .HasName("IX_tblUser")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(25);

                entity.Property(e => e.EmployeeCode).HasMaxLength(20);

                entity.Property(e => e.MailAddress).HasMaxLength(100);

                entity.Property(e => e.MailPassword).HasMaxLength(50);

                entity.Property(e => e.Modifyer).HasMaxLength(25);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.EmployeeCodeNavigation)
                    .WithMany(p => p.TblUser)
                    .HasForeignKey(d => d.EmployeeCode)
                    .HasConstraintName("FK_tblUser_V_Employee");
            });

            modelBuilder.Entity<TblUserHasCompany>(entity =>
            {
                entity.HasKey(e => e.UserHasCompanyId)
                    .HasName("PK_tblUserHasCompany");

                entity.ToTable("tblUserHasCompany", "company");

                entity.Property(e => e.UserHasCompanyId).HasColumnName("UserHasCompanyID");

                entity.Property(e => e.CompanyId).HasColumnName("CompanyID");

                entity.Property(e => e.Creator).HasMaxLength(20);

                entity.Property(e => e.Modifyer).HasMaxLength(20);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TblUserHasCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_tblUserHasCompany_tblCompany");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TblUserHasCompany)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_tblUserHasCompany_tblUser");
            });

            modelBuilder.Entity<VEmployee>(entity =>
            {
                entity.HasKey(e => e.EmpCode)
                    .HasName("PK_V_Employee");

                entity.ToTable("V_Employee");

                entity.Property(e => e.EmpCode).HasMaxLength(20);

                entity.Property(e => e.NameEng).HasMaxLength(100);

                entity.Property(e => e.NameThai).HasMaxLength(100);

                entity.Property(e => e.SectionCode).HasMaxLength(20);

                entity.Property(e => e.SectionName).HasMaxLength(100);
            });
        }
    }
}