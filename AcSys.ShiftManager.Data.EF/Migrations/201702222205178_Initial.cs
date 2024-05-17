namespace AcSys.ShiftManager.Data.EF.Configuration
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityLog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DateTimeStamp = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        SubjectType = c.Int(nullable: false),
                        SubjectId = c.Guid(),
                        SubjectSnapshot = c.String(),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        User_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        WorkNumber = c.String(),
                        Location = c.String(),
                        Address = c.String(),
                        PostCode = c.String(),
                        City = c.String(),
                        HasDrivingLicense = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        Email = c.String(nullable: false, maxLength: 75),
                        EmailConfirmed = c.Boolean(nullable: false),
                        LockoutEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        PasswordHash = c.String(maxLength: 200),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        SecurityStamp = c.String(maxLength: 200),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 75),
                        MustChangePassword = c.Boolean(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 25),
                        LastName = c.String(nullable: false, maxLength: 25),
                        DateOfBirth = c.DateTime(nullable: false),
                        Mobile = c.String(nullable: false, maxLength: 20),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        EmployeeGroup_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmployeeGroup", t => t.EmployeeGroup_Id)
                .Index(t => t.EmployeeGroup_Id);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Text = c.String(nullable: false),
                        SentAt = c.DateTime(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        Type = c.Int(nullable: false),
                        SubjectId = c.Guid(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        Sender_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.Sender_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.NotificationView",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ViewedAt = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        User_Id = c.Guid(nullable: false),
                        Notification_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .ForeignKey("dbo.Notification", t => t.Notification_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Notification_Id);
            
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        ClaimType = c.String(nullable: false, maxLength: 100),
                        ClaimValue = c.String(nullable: false, maxLength: 100),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.EmployeeGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Subject = c.String(nullable: false),
                        Text = c.String(nullable: false),
                        SentAt = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        Sender_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.Sender_Id)
                .Index(t => t.Sender_Id);
            
            CreateTable(
                "dbo.MessageView",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ViewedAt = c.DateTime(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        User_Id = c.Guid(nullable: false),
                        Message_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .ForeignKey("dbo.Message", t => t.Message_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Message_Id);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        Id = c.Guid(nullable: false),
                        Timestamp = c.Binary(),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Shift",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsOpen = c.Boolean(nullable: false),
                        Title = c.String(nullable: false),
                        Notes = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        ClockInTime = c.DateTime(),
                        ClockOutTime = c.DateTime(),
                        TotalBreakMins = c.Int(nullable: false),
                        TotalMins = c.Int(nullable: false),
                        LateMins = c.Int(nullable: false),
                        ClockedMins = c.Int(nullable: false),
                        ShortMins = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StartStatus = c.Int(nullable: false),
                        EndStatus = c.Int(nullable: false),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        Employee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.Employee_Id)
                .Index(t => t.Employee_Id);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.Role", t => t.RoleId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Timestamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        EntityStatus = c.Int(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NotificationUser",
                c => new
                    {
                        Notification_Id = c.Guid(nullable: false),
                        User_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Notification_Id, t.User_Id })
                .ForeignKey("dbo.Notification", t => t.Notification_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Notification_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.MessageUser",
                c => new
                    {
                        Message_Id = c.Guid(nullable: false),
                        User_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Message_Id, t.User_Id })
                .ForeignKey("dbo.Message", t => t.Message_Id)
                .ForeignKey("dbo.User", t => t.User_Id)
                .Index(t => t.Message_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityLog", "User_Id", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.Shift", "Employee_Id", "dbo.User");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.MessageView", "Message_Id", "dbo.Message");
            DropForeignKey("dbo.MessageView", "User_Id", "dbo.User");
            DropForeignKey("dbo.Message", "Sender_Id", "dbo.User");
            DropForeignKey("dbo.MessageUser", "User_Id", "dbo.User");
            DropForeignKey("dbo.MessageUser", "Message_Id", "dbo.Message");
            DropForeignKey("dbo.User", "EmployeeGroup_Id", "dbo.EmployeeGroup");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropForeignKey("dbo.NotificationView", "Notification_Id", "dbo.Notification");
            DropForeignKey("dbo.NotificationView", "User_Id", "dbo.User");
            DropForeignKey("dbo.Notification", "Sender_Id", "dbo.User");
            DropForeignKey("dbo.NotificationUser", "User_Id", "dbo.User");
            DropForeignKey("dbo.NotificationUser", "Notification_Id", "dbo.Notification");
            DropIndex("dbo.MessageUser", new[] { "User_Id" });
            DropIndex("dbo.MessageUser", new[] { "Message_Id" });
            DropIndex("dbo.NotificationUser", new[] { "User_Id" });
            DropIndex("dbo.NotificationUser", new[] { "Notification_Id" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.Shift", new[] { "Employee_Id" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.MessageView", new[] { "Message_Id" });
            DropIndex("dbo.MessageView", new[] { "User_Id" });
            DropIndex("dbo.Message", new[] { "Sender_Id" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.NotificationView", new[] { "Notification_Id" });
            DropIndex("dbo.NotificationView", new[] { "User_Id" });
            DropIndex("dbo.Notification", new[] { "Sender_Id" });
            DropIndex("dbo.User", new[] { "EmployeeGroup_Id" });
            DropIndex("dbo.ActivityLog", new[] { "User_Id" });
            DropTable("dbo.MessageUser");
            DropTable("dbo.NotificationUser");
            DropTable("dbo.Role");
            DropTable("dbo.UserRole");
            DropTable("dbo.Shift");
            DropTable("dbo.UserLogin");
            DropTable("dbo.MessageView");
            DropTable("dbo.Message");
            DropTable("dbo.EmployeeGroup");
            DropTable("dbo.UserClaim");
            DropTable("dbo.NotificationView");
            DropTable("dbo.Notification");
            DropTable("dbo.User");
            DropTable("dbo.ActivityLog");
        }
    }
}
