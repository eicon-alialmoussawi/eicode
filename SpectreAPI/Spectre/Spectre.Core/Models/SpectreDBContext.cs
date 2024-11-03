using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Spectre.Core.Models
{
    public partial class SpectreDBContext : IdentityDbContext
    {
        public SpectreDBContext()
        {
        }

        public SpectreDBContext(DbContextOptions<SpectreDBContext> options)
            : base(options)
        {
        }
        public virtual DbSet<AboutSpectre> AboutSpectres { get; set; }
        public virtual DbSet<ActionLogOperation> ActionLogOperations { get; set; }
        //public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        //public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        //public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        //public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        //public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        //public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        //public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<AspnetPermission> AspnetPermissions { get; set; }
        public virtual DbSet<Award> Awards { get; set; }
        public virtual DbSet<Band> Bands { get; set; }
        public virtual DbSet<Banner> Banners { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyPackage> CompanyPackages { get; set; }
        public virtual DbSet<CompanyPackageDetail> CompanyPackageDetails { get; set; }
        public virtual DbSet<CompanyPreRegistration> CompanyPreRegistrations { get; set; }
        public virtual DbSet<ContactU> ContactUs { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Fqa> Fqas { get; set; }
        public virtual DbSet<Glossary> Glossaries { get; set; }
        public virtual DbSet<HelpAboutU> HelpAboutUs { get; set; }
        public virtual DbSet<HelpService> HelpServices { get; set; }
        public virtual DbSet<HelpUsing> HelpUsings { get; set; }
        public virtual DbSet<HomePageHeader> HomePageHeaders { get; set; }
        public virtual DbSet<Icon> Icons { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LatestNews> LatestNews { get; set; }
        public virtual DbSet<Lookup> Lookups { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackagePagePermission> PackagePagePermissions { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<RegisterationRequest> RegisterationRequests { get; set; }
        public virtual DbSet<ReportSnap> ReportSnaps { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<SavedFilter> SavedFilters { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Sheet1> Sheet1s { get; set; }
        public virtual DbSet<SocioEonomic> SocioEonomics { get; set; }
        public virtual DbSet<SpectreFooter> SpectreFooters { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<TemplateFilter> TemplateFilters { get; set; }
        public virtual DbSet<TemplateFilterDetail> TemplateFilterDetails { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserActionLog> UserActionLogs { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }
        public virtual DbSet<VisualizingReport> VisualizingReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=95.216.139.227,50666;Database=SpectreDBTest; User Id=m.itani;password=SQL@ids#2021; Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1256_CI_AS");

            modelBuilder.Entity<AboutSpectre>(entity =>
            {
                entity.Property(e => e.TitleAr).HasMaxLength(800);

                entity.Property(e => e.TitleEn).HasMaxLength(800);

                entity.Property(e => e.TitleFr).HasMaxLength(800);
            });

            modelBuilder.Entity<ActionLogOperation>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.Stamp)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspnetPermission>(entity =>
            {
                entity.ToTable("aspnet_Permissions");

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ActionAr).HasMaxLength(50);

                entity.Property(e => e.GroupNameAr)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.GroupNameEn)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.IconClass)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MenuNameAr)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.MenuNameEn)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NameAr)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PageUrl)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("PageURL");
            });

            modelBuilder.Entity<Award>(entity =>
            {
                entity.HasIndex(e => e.Year, "AwardYear");

                entity.HasIndex(e => e.CountryId, "CountryId");

                entity.Property(e => e.AnnualFees).HasColumnType("numeric(18, 6)");

                entity.Property(e => e.BandPaired)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.BandType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.BandUnPaired)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.BlockPaired)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.BlockUnPaired)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Operator)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Pop).HasColumnType("numeric(18, 6)");

                entity.Property(e => e.PricePaid).HasColumnType("numeric(18, 6)");

                entity.Property(e => e.ReservePrice).HasColumnType("numeric(18, 6)");

                entity.Property(e => e.SingleOrmultiBand)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("SingleORMultiBand");

                entity.Property(e => e.Terms)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.UpFrontFees).HasColumnType("numeric(18, 6)");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Awards)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__Awards__CountryI__75A278F5");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Awards)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Awards__CreatedB__76969D2E");

                entity.HasOne(d => d.OperatorNavigation)
                    .WithMany(p => p.AwardOperatorNavigations)
                    .HasForeignKey(d => d.OperatorId)
                    .HasConstraintName("FK__Awards__Operator__778AC167");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.AwardSources)
                    .HasForeignKey(d => d.SourceId)
                    .HasConstraintName("FK__Awards__SourceId__787EE5A0");
            });

            modelBuilder.Entity<Band>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.TitleAr).HasMaxLength(300);

                entity.Property(e => e.TitleEn)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Bands)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Bands__CreatedBy__797309D9");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(300)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Banners)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Banners__Created__7A672E12");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(800);

                entity.Property(e => e.Name).HasMaxLength(800);

                entity.Property(e => e.PhoneNumber).HasMaxLength(800);
            });

            modelBuilder.Entity<CompanyPackage>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 1)");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.CompanyPackages)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__CompanyPa__Compa__1BC821DD");

                entity.HasOne(d => d.CurrencyNavigation)
                    .WithMany(p => p.CompanyPackages)
                    .HasForeignKey(d => d.Currency)
                    .HasConstraintName("FK__CompanyPa__Curre__1CBC4616");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.CompanyPackages)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("FK__CompanyPa__Packa__1AD3FDA4");
            });

            modelBuilder.Entity<CompanyPackageDetail>(entity =>
            {
                entity.HasOne(d => d.CompanyPackage)
                    .WithMany(p => p.CompanyPackageDetails)
                    .HasForeignKey(d => d.CompanyPackageId)
                    .HasConstraintName("FK__CompanyPa__Compa__1F98B2C1");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.CompanyPackageDetails)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__CompanyPa__Count__2180FB33");

                entity.HasOne(d => d.PackagePagePermission)
                    .WithMany(p => p.CompanyPackageDetails)
                    .HasForeignKey(d => d.PackagePagePermissionId)
                    .HasConstraintName("FK__CompanyPa__Packa__208CD6FA");
            });

            modelBuilder.Entity<CompanyPreRegistration>(entity =>
            {
                entity.ToTable("CompanyPreRegistration");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(800);

                entity.Property(e => e.Name).HasMaxLength(800);

                entity.Property(e => e.PhoneNumber).HasMaxLength(800);

                entity.HasOne(d => d.PreferredPackageNavigation)
                    .WithMany(p => p.CompanyPreRegistrations)
                    .HasForeignKey(d => d.PreferredPackage)
                    .HasConstraintName("FK__CompanyPr__Prefe__160F4887");
            });

            //added the line below to allow triggers
            modelBuilder.Entity<CompanyPreRegistration>().ToTable(tb => tb.HasTrigger("tableTrigger"));

            modelBuilder.Entity<ContactU>(entity =>
            {
                entity.Property(e => e.CompanyName).HasMaxLength(800);

                entity.Property(e => e.Email).HasMaxLength(800);

                entity.Property(e => e.Name).HasMaxLength(800);

                entity.Property(e => e.PhoneNumber).HasMaxLength(800);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.ContactUs)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__ContactUs__Count__6442E2C9");
            });

            //added the line below to allow triggers
            modelBuilder.Entity<ContactU>().ToTable(tb => tb.HasTrigger("contactTrigger"));

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId)
                    .ValueGeneratedNever()
                    .HasColumnName("country_id");

                entity.Property(e => e.Iso3)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("iso3");

                entity.Property(e => e.NameAr)
                    .HasMaxLength(800)
                    .HasColumnName("name_ar");

                entity.Property(e => e.NameEn)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name_en");
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.Property(e => e.ExceptionDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.Property(e => e.TitleAr).HasMaxLength(800);

                entity.Property(e => e.TitleEn).HasMaxLength(800);

                entity.Property(e => e.TitleFr).HasMaxLength(800);
            });

            modelBuilder.Entity<Fqa>(entity =>
            {
                entity.ToTable("FQA");
            });

            modelBuilder.Entity<Glossary>(entity =>
            {
                entity.ToTable("Glossary");

                entity.Property(e => e.DescriptionAr).HasMaxLength(800);

                entity.Property(e => e.DescriptionEn).HasMaxLength(800);

                entity.Property(e => e.DescriptionFr).HasMaxLength(800);

                entity.Property(e => e.NameAr).HasMaxLength(800);

                entity.Property(e => e.NameEn).HasMaxLength(800);

                entity.Property(e => e.NameFr).HasMaxLength(800);
            });

            modelBuilder.Entity<HelpAboutU>(entity =>
            {
                entity.Property(e => e.TitleAr).HasMaxLength(800);

                entity.Property(e => e.TitleEn).HasMaxLength(800);

                entity.Property(e => e.TitleFr).HasMaxLength(800);
            });

            modelBuilder.Entity<HelpService>(entity =>
            {
                entity.Property(e => e.Icon).HasMaxLength(800);

                entity.Property(e => e.TitleAr).HasMaxLength(400);

                entity.Property(e => e.TitleEn).HasMaxLength(400);

                entity.Property(e => e.TitleFr).HasMaxLength(400);
            });

            modelBuilder.Entity<HelpUsing>(entity =>
            {
                entity.ToTable("HelpUsing");

                entity.Property(e => e.DescriptionAr).HasMaxLength(800);

                entity.Property(e => e.DescriptionEn).HasMaxLength(800);

                entity.Property(e => e.DescriptionFr).HasMaxLength(800);

                entity.Property(e => e.TitleAr).HasMaxLength(800);

                entity.Property(e => e.TitleEn).HasMaxLength(800);

                entity.Property(e => e.TitleFr).HasMaxLength(800);
            });

            modelBuilder.Entity<HomePageHeader>(entity =>
            {
                entity.Property(e => e.DescriptionAr).HasMaxLength(800);

                entity.Property(e => e.DescriptionEn).HasMaxLength(800);

                entity.Property(e => e.DescriptionFr).HasMaxLength(800);

                entity.Property(e => e.Title1Ar).HasMaxLength(800);

                entity.Property(e => e.Title1En).HasMaxLength(800);

                entity.Property(e => e.Title1Fr).HasMaxLength(800);

                entity.Property(e => e.Title2Ar).HasMaxLength(800);

                entity.Property(e => e.Title2En).HasMaxLength(800);

                entity.Property(e => e.Title2Fr).HasMaxLength(800);
            });

            modelBuilder.Entity<Icon>(entity =>
            {
                entity.Property(e => e.Icon1)
                    .HasMaxLength(800)
                    .HasColumnName("Icon");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Code).HasMaxLength(300);

                entity.Property(e => e.Langauge).HasMaxLength(800);
            });

            modelBuilder.Entity<LatestNews>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.DescriptionAr).IsUnicode(false);

                entity.Property(e => e.DescriptionEn).IsUnicode(false);

                entity.Property(e => e.DescriptionFr).HasMaxLength(800);

                entity.Property(e => e.ImageUrl)
                    .IsUnicode(false)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.TitleAr)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.TitleEn)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.TitleFr).HasMaxLength(800);

                entity.Property(e => e.Url)
                    .IsUnicode(false)
                    .HasColumnName("URL");
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.LookupCode)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ModiciationDate).HasColumnType("smalldatetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.NameAr)
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

                entity.Property(e => e.NameEn).HasMaxLength(800);

                entity.Property(e => e.NameFr).HasMaxLength(800);

                entity.Property(e => e.NameSpa).HasMaxLength(800);
            });

            modelBuilder.Entity<PackagePagePermission>(entity =>
            {
                entity.Property(e => e.PageUrl)
                    .HasMaxLength(800)
                    .HasColumnName("PageURL");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.PackagePagePermissions)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("FK__PackagePa__Packa__1332DBDC");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(800);

                entity.Property(e => e.NameAr).HasMaxLength(800);

                entity.Property(e => e.NameFr).HasMaxLength(800);

                entity.Property(e => e.PageUrl).HasMaxLength(800);
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.Property(e => e.NameAr).HasMaxLength(800);

                entity.Property(e => e.NameEn).HasMaxLength(800);

                entity.Property(e => e.NameFr).HasMaxLength(800);

                entity.Property(e => e.Value).HasMaxLength(800);
            });

            modelBuilder.Entity<RegisterationRequest>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(300);

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(300);

                entity.Property(e => e.Phone).HasMaxLength(300);

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.RegisterationRequests)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__Registera__Statu__7B5B524B");
            });

            modelBuilder.Entity<ReportSnap>(entity =>
            {
                entity.Property(e => e.ImageUrl).HasColumnName("ImageURL");

                entity.Property(e => e.ReferenceText).HasMaxLength(800);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("Role_Permission");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Role_Permission_aspnet_Permissions");
            });

            modelBuilder.Entity<SavedFilter>(entity =>
            {
                entity.Property(e => e.PageUrl).HasMaxLength(800);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SavedFilters)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__SavedFilt__UserI__0B5CAFEA");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Icon).HasMaxLength(800);

                entity.Property(e => e.TitleAr).HasMaxLength(400);

                entity.Property(e => e.TitleEn).HasMaxLength(400);

                entity.Property(e => e.TitleFr).HasMaxLength(400);
            });

            modelBuilder.Entity<Sheet1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Sheet1$");

                entity.Property(e => e.Iso).HasMaxLength(255);

                entity.Property(e => e.NameAr).HasMaxLength(255);
            });

            modelBuilder.Entity<SocioEonomic>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.CountryId, e.SourceId }, "SocioEcnomicFilter");

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .HasColumnType("decimal(28, 6)")
                    .HasColumnName("value");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.SocioEonomics)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__SocioEono__Count__7D439ABD");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.SocioEonomics)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__SocioEono__Creat__7E37BEF6");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.SocioEonomicSources)
                    .HasForeignKey(d => d.SourceId)
                    .HasConstraintName("FK__SocioEono__Sourc__7F2BE32F");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.SocioEonomicTypes)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__SocioEono__TypeI__00200768");
            });

            modelBuilder.Entity<SpectreFooter>(entity =>
            {
                entity.Property(e => e.AddressAr).HasMaxLength(800);

                entity.Property(e => e.AddressEn).HasMaxLength(800);

                entity.Property(e => e.AddressFr).HasMaxLength(800);

                entity.Property(e => e.BuildingAr).HasMaxLength(800);

                entity.Property(e => e.BuildingEn).HasMaxLength(800);

                entity.Property(e => e.BuildingFr).HasMaxLength(800);

                entity.Property(e => e.CityAr).HasMaxLength(800);

                entity.Property(e => e.CityEn).HasMaxLength(800);

                entity.Property(e => e.CityFr).HasMaxLength(800);

                entity.Property(e => e.Email).HasMaxLength(800);

                entity.Property(e => e.PhoneNumber).HasMaxLength(800);

                entity.Property(e => e.StreetAr).HasMaxLength(800);

                entity.Property(e => e.StreetEn).HasMaxLength(800);

                entity.Property(e => e.StreetFr).HasMaxLength(800);

                entity.Property(e => e.TitleAr).HasMaxLength(800);

                entity.Property(e => e.TitleEn).HasMaxLength(800);

                entity.Property(e => e.TitleFr).HasMaxLength(800);
            });

            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AboutAr).IsUnicode(false);

                entity.Property(e => e.AboutEn).IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.AddressAr)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Fax).IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.WelcomeMessage)
                    .HasMaxLength(800)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TemplateFilter>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.PageName)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TemplateFilters)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__TemplateF__Creat__02FC7413");
            });

            modelBuilder.Entity<TemplateFilterDetail>(entity =>
            {
                entity.Property(e => e.ControlName)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ControlType)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.ControlValue).IsUnicode(false);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TemplateFilterDetails)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__TemplateF__Creat__01142BA1");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateFilterDetails)
                    .HasForeignKey(d => d.TemplateId)
                    .HasConstraintName("FK__TemplateF__Templ__02084FDA");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnType("datetime")
                    .HasColumnName("DOB");

                entity.Property(e => e.Email)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.IdentityNumber)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.IdentityUserId)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.JobTitle).HasMaxLength(800);

                entity.Property(e => e.LastName)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.UserType).HasMaxLength(800);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK__Users__CompanyId__22751F6C");
            });

            modelBuilder.Entity<UserActionLog>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Details)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.DetailsAr)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.Page)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.PageAr)
                    .HasMaxLength(800)
                    .IsUnicode(false);

                entity.Property(e => e.Stamp)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserActionLogs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserActionLogs_UserActionLogs");
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.Key).HasMaxLength(800);

                entity.Property(e => e.TextAr).HasMaxLength(800);

                entity.Property(e => e.TextEn).HasMaxLength(800);

                entity.Property(e => e.TextFr).HasMaxLength(800);

                entity.Property(e => e.Url)
                    .HasMaxLength(800)
                    .HasColumnName("URL");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserNotifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserNotif__UserI__3587F3E0");
            });

            modelBuilder.Entity<VisualizingReport>(entity =>
            {
                entity.Property(e => e.TitleAr).HasMaxLength(400);

                entity.Property(e => e.TitleEn).HasMaxLength(400);

                entity.Property(e => e.TitleFr).HasMaxLength(400);
            });

            base.OnModelCreating(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
