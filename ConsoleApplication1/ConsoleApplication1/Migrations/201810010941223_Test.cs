namespace DLL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessCodeValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccessCode = c.String(),
                        AccessValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AccessGroups",
                c => new
                    {
                        AccessGroupId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AccessGroupId);
            
            CreateTable(
                "dbo.AttendanceRemarksRegisters",
                c => new
                    {
                        AttendanceRemarksRegisterId = c.Int(nullable: false, identity: true),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        remarks_r_id = c.Int(),
                        active = c.Boolean(nullable: false),
                        Remark_RemarkId = c.Int(),
                    })
                .PrimaryKey(t => t.AttendanceRemarksRegisterId)
                .ForeignKey("dbo.Remarks", t => t.Remark_RemarkId)
                .Index(t => t.Remark_RemarkId);
            
            CreateTable(
                "dbo.Remarks",
                c => new
                    {
                        RemarkId = c.Int(nullable: false, identity: true),
                        description = c.String(),
                        short_hand = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RemarkId);
            
            CreateTable(
                "dbo.AuditTrails",
                c => new
                    {
                        AuditTrailId = c.Int(nullable: false, identity: true),
                        action_code = c.Int(nullable: false),
                        description = c.String(),
                        table = c.String(),
                        entity_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AuditTrailId);
            
            CreateTable(
                "dbo.BankNames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BankNameText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConsolidatedAttendances",
                c => new
                    {
                        ConsolidatedAttendanceId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        status_in = c.String(),
                        status_out = c.String(),
                        final_remarks = c.String(),
                        terminal_in = c.String(),
                        terminal_out = c.String(),
                        active = c.Boolean(nullable: false),
                        overtime = c.Int(nullable: false),
                        overtime_status = c.Int(nullable: false),
                        employee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.ConsolidatedAttendanceId)
                .ForeignKey("dbo.Employees", t => t.employee_EmployeeId)
                .Index(t => t.employee_EmployeeId);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        first_name = c.String(),
                        last_name = c.String(),
                        employee_code = c.String(maxLength: 50),
                        email = c.String(),
                        address = c.String(),
                        mobile_no = c.String(),
                        date_of_joining = c.DateTime(),
                        date_of_leaving = c.DateTime(),
                        active = c.Boolean(nullable: false),
                        timetune_active = c.Boolean(nullable: false),
                        date_of_birth = c.DateTime(),
                        skills_set = c.String(),
                        password = c.String(),
                        salt = c.String(),
                        sick_leaves = c.Int(nullable: false),
                        casual_leaves = c.Int(nullable: false),
                        annual_leaves = c.Int(nullable: false),
                        photograph = c.String(),
                        file_01 = c.String(),
                        file_02 = c.String(),
                        file_03 = c.String(),
                        file_04 = c.String(),
                        file_05 = c.String(),
                        emergency_contact_01 = c.String(),
                        emergency_contact_02 = c.String(),
                        description = c.String(),
                        access_group_AccessGroupId = c.Int(),
                        department_DepartmentId = c.Int(),
                        designation_DesignationId = c.Int(),
                        function_FunctionId = c.Int(),
                        grade_GradeId = c.Int(),
                        Group_GroupId = c.Int(),
                        location_LocationId = c.Int(),
                        region_RegionId = c.Int(),
                        type_of_employment_TypeOfEmploymentId = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeId)
                .ForeignKey("dbo.AccessGroups", t => t.access_group_AccessGroupId)
                .ForeignKey("dbo.Departments", t => t.department_DepartmentId)
                .ForeignKey("dbo.Designations", t => t.designation_DesignationId)
                .ForeignKey("dbo.Functions", t => t.function_FunctionId)
                .ForeignKey("dbo.Grades", t => t.grade_GradeId)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId)
                .ForeignKey("dbo.Locations", t => t.location_LocationId)
                .ForeignKey("dbo.Regions", t => t.region_RegionId)
                .ForeignKey("dbo.TypeOfEmployments", t => t.type_of_employment_TypeOfEmploymentId)
                .Index(t => t.employee_code, unique: true)
                .Index(t => t.access_group_AccessGroupId)
                .Index(t => t.department_DepartmentId)
                .Index(t => t.designation_DesignationId)
                .Index(t => t.function_FunctionId)
                .Index(t => t.grade_GradeId)
                .Index(t => t.Group_GroupId)
                .Index(t => t.location_LocationId)
                .Index(t => t.region_RegionId)
                .Index(t => t.type_of_employment_TypeOfEmploymentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Designations",
                c => new
                    {
                        DesignationId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DesignationId);
            
            CreateTable(
                "dbo.Functions",
                c => new
                    {
                        FunctionId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FunctionId);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        GradeId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GradeId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        group_name = c.String(),
                        group_description = c.String(),
                        follows_general_calendar = c.Boolean(nullable: false),
                        active = c.Boolean(nullable: false),
                        supervisor_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.GroupCalendars",
                c => new
                    {
                        GroupCalendarId = c.Int(nullable: false, identity: true),
                        year = c.Int(nullable: false),
                        name = c.String(),
                        active = c.Boolean(nullable: false),
                        friday_ShiftId = c.Int(),
                        generalShift_ShiftId = c.Int(),
                        Group_GroupId = c.Int(),
                        monday_ShiftId = c.Int(),
                        saturday_ShiftId = c.Int(),
                        sunday_ShiftId = c.Int(),
                        thursday_ShiftId = c.Int(),
                        tuesday_ShiftId = c.Int(),
                        wednesday_ShiftId = c.Int(),
                    })
                .PrimaryKey(t => t.GroupCalendarId)
                .ForeignKey("dbo.Shifts", t => t.friday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.generalShift_ShiftId)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId)
                .ForeignKey("dbo.Shifts", t => t.monday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.saturday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.sunday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.thursday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.tuesday_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.wednesday_ShiftId)
                .Index(t => t.friday_ShiftId)
                .Index(t => t.generalShift_ShiftId)
                .Index(t => t.Group_GroupId)
                .Index(t => t.monday_ShiftId)
                .Index(t => t.saturday_ShiftId)
                .Index(t => t.sunday_ShiftId)
                .Index(t => t.thursday_ShiftId)
                .Index(t => t.tuesday_ShiftId)
                .Index(t => t.wednesday_ShiftId);
            
            CreateTable(
                "dbo.GroupCalendarOverrides",
                c => new
                    {
                        GroupCalendarOverrideId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(),
                        reason = c.String(),
                        isGazettedHoliday = c.Boolean(nullable: false),
                        active = c.Boolean(nullable: false),
                        GroupCalendar_GroupCalendarId = c.Int(),
                        Shift_ShiftId = c.Int(),
                    })
                .PrimaryKey(t => t.GroupCalendarOverrideId)
                .ForeignKey("dbo.GroupCalendars", t => t.GroupCalendar_GroupCalendarId)
                .ForeignKey("dbo.Shifts", t => t.Shift_ShiftId)
                .Index(t => t.GroupCalendar_GroupCalendarId)
                .Index(t => t.Shift_ShiftId);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        ShiftId = c.Int(nullable: false, identity: true),
                        early_time = c.Int(nullable: false),
                        start_time = c.DateTime(nullable: false),
                        late_time = c.Int(nullable: false),
                        half_day = c.Int(nullable: false),
                        shift_end = c.Int(nullable: false),
                        day_end = c.Int(nullable: false),
                        name = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ShiftId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LocationId);
            
            CreateTable(
                "dbo.PersistentAttendanceLogs",
                c => new
                    {
                        PersistentAttendanceLogId = c.Int(nullable: false),
                        employee_code = c.String(maxLength: 50),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        time_start = c.DateTime(),
                        time_end = c.DateTime(),
                        start_time = c.DateTime(),
                        late_time = c.DateTime(),
                        half_day = c.DateTime(),
                        shift_end = c.DateTime(),
                        terminal_in = c.String(),
                        terminal_out = c.String(),
                        dirtyBit = c.Boolean(nullable: false),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PersistentAttendanceLogId)
                .ForeignKey("dbo.Employees", t => t.PersistentAttendanceLogId)
                .Index(t => t.PersistentAttendanceLogId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        RegionId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RegionId);
            
            CreateTable(
                "dbo.TypeOfEmployments",
                c => new
                    {
                        TypeOfEmploymentId = c.Int(nullable: false, identity: true),
                        dbID = c.Int(nullable: false),
                        name = c.String(),
                        description = c.String(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TypeOfEmploymentId);
            
            CreateTable(
                "dbo.ManualAttendances",
                c => new
                    {
                        ManualAttendanceId = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        remarks = c.String(),
                        ConsolidatedAttendance_ConsolidatedAttendanceId = c.Int(),
                        employee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.ManualAttendanceId)
                .ForeignKey("dbo.ConsolidatedAttendances", t => t.ConsolidatedAttendance_ConsolidatedAttendanceId)
                .ForeignKey("dbo.Employees", t => t.employee_EmployeeId)
                .Index(t => t.ConsolidatedAttendance_ConsolidatedAttendanceId)
                .Index(t => t.employee_EmployeeId);
            
            CreateTable(
                "dbo.ConsolidatedAttendancesArchives",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ConsolidatedAttendanceId = c.Int(nullable: false),
                        date = c.DateTime(),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        status_in = c.String(),
                        status_out = c.String(),
                        final_remarks = c.String(),
                        terminal_in = c.String(),
                        terminal_out = c.String(),
                        active = c.Boolean(nullable: false),
                        employee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Employees", t => t.employee_EmployeeId)
                .Index(t => t.employee_EmployeeId);
            
            CreateTable(
                "dbo.ContractualStaffs",
                c => new
                    {
                        ContractualStaffId = c.Int(nullable: false, identity: true),
                        employee_code = c.String(maxLength: 50),
                        employee_name = c.String(),
                        email = c.String(),
                        address = c.String(),
                        mobile_no = c.String(),
                        active = c.Boolean(nullable: false),
                        department = c.String(),
                        designation = c.String(),
                        function = c.String(),
                        grade = c.String(),
                        Group = c.String(),
                        location = c.String(),
                        region = c.String(),
                        company = c.String(),
                        date_of_joining = c.DateTime(),
                        date_of_leaving = c.DateTime(),
                    })
                .PrimaryKey(t => t.ContractualStaffId)
                .Index(t => t.employee_code, unique: true);
            
            CreateTable(
                "dbo.CS_AttendanceLog",
                c => new
                    {
                        CS_AttendanceLogId = c.Int(nullable: false),
                        employee_code = c.String(maxLength: 50),
                        date = c.DateTime(),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        terminal_in = c.String(),
                        terminal_out = c.String(),
                        dirtyBit = c.Boolean(nullable: false),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CS_AttendanceLogId)
                .ForeignKey("dbo.ContractualStaffs", t => t.CS_AttendanceLogId)
                .Index(t => t.CS_AttendanceLogId);
            
            CreateTable(
                "dbo.CS_PersistentLog",
                c => new
                    {
                        CS_PersistentLogId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(),
                        time_in = c.DateTime(),
                        time_out = c.DateTime(),
                        terminal_in = c.String(),
                        terminal_out = c.String(),
                        remarks = c.String(),
                        active = c.Boolean(nullable: false),
                        employee_ContractualStaffId = c.Int(),
                    })
                .PrimaryKey(t => t.CS_PersistentLogId)
                .ForeignKey("dbo.ContractualStaffs", t => t.employee_ContractualStaffId)
                .Index(t => t.employee_ContractualStaffId);
            
            CreateTable(
                "dbo.EmployeeEvaluations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        empCode = c.String(),
                        reviewPeriod = c.String(),
                        postion = c.String(),
                        project = c.String(),
                        primaryResponsibilities = c.String(),
                        secondaryResponsibilities = c.String(),
                        careerPath = c.String(),
                        personality = c.Int(nullable: false),
                        communicationSkills = c.Int(nullable: false),
                        attendancePromptness = c.Int(nullable: false),
                        imitative = c.Int(nullable: false),
                        organizationAwareness = c.Int(nullable: false),
                        selfControl = c.Int(nullable: false),
                        proficiency = c.Int(nullable: false),
                        projectManagement = c.Int(nullable: false),
                        attentionDetail = c.Int(nullable: false),
                        clientInteraction = c.Int(nullable: false),
                        creativity = c.Int(nullable: false),
                        businessSkill = c.Int(nullable: false),
                        achievement = c.Int(nullable: false),
                        majorStrength = c.String(),
                        areaImprovement = c.String(),
                        otherComment = c.String(),
                        goal = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Letters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LetterName = c.String(),
                        formate = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EmployeeForgotPasswords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeCode = c.String(),
                        GuidCode = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FutureManualAttendances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        from_date = c.DateTime(nullable: false),
                        to_date = c.DateTime(nullable: false),
                        employee_code = c.String(),
                        remarks = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GeneralCalendars",
                c => new
                    {
                        GeneralCalendarId = c.Int(nullable: false, identity: true),
                        year = c.Int(nullable: false),
                        name = c.String(),
                        active = c.Boolean(nullable: false),
                        generalShift_ShiftId = c.Int(),
                        Shift_ShiftId = c.Int(),
                        Shift1_ShiftId = c.Int(),
                        Shift2_ShiftId = c.Int(),
                        Shift3_ShiftId = c.Int(),
                        Shift4_ShiftId = c.Int(),
                        Shift5_ShiftId = c.Int(),
                        Shift6_ShiftId = c.Int(),
                    })
                .PrimaryKey(t => t.GeneralCalendarId)
                .ForeignKey("dbo.Shifts", t => t.generalShift_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift1_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift2_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift3_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift4_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift5_ShiftId)
                .ForeignKey("dbo.Shifts", t => t.Shift6_ShiftId)
                .Index(t => t.generalShift_ShiftId)
                .Index(t => t.Shift_ShiftId)
                .Index(t => t.Shift1_ShiftId)
                .Index(t => t.Shift2_ShiftId)
                .Index(t => t.Shift3_ShiftId)
                .Index(t => t.Shift4_ShiftId)
                .Index(t => t.Shift5_ShiftId)
                .Index(t => t.Shift6_ShiftId);
            
            CreateTable(
                "dbo.GeneralCalendarOverrides",
                c => new
                    {
                        GeneralCalendarOverrideId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(),
                        reason = c.String(),
                        isGazettedHoliday = c.Boolean(nullable: false),
                        active = c.Boolean(nullable: false),
                        Shift_ShiftId = c.Int(),
                        GeneralCalendar_GeneralCalendarId = c.Int(),
                    })
                .PrimaryKey(t => t.GeneralCalendarOverrideId)
                .ForeignKey("dbo.Shifts", t => t.Shift_ShiftId)
                .ForeignKey("dbo.GeneralCalendars", t => t.GeneralCalendar_GeneralCalendarId)
                .Index(t => t.Shift_ShiftId)
                .Index(t => t.GeneralCalendar_GeneralCalendarId);
            
            CreateTable(
                "dbo.HaTransits",
                c => new
                    {
                        HaTransitId = c.Int(nullable: false, identity: true),
                        C_Name = c.String(),
                        C_Unique = c.String(),
                        C_Date = c.DateTime(),
                        C_Time = c.String(),
                        L_UID = c.Int(nullable: false),
                        L_TID = c.Int(),
                        active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HaTransitId);
            
            CreateTable(
                "dbo.LeaveApplications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LeaveTypeId = c.Int(nullable: false),
                        FromDate = c.DateTime(nullable: false),
                        ToDate = c.DateTime(nullable: false),
                        DaysCount = c.Int(nullable: false),
                        LeaveReasonId = c.Int(nullable: false),
                        ReasonDetail = c.String(),
                        ApproverId = c.Int(nullable: false),
                        ApproverDetail = c.String(),
                        LeaveStatusId = c.Int(nullable: false),
                        AttachmentFilePath = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LeaveReasons", t => t.LeaveReasonId, cascadeDelete: true)
                .ForeignKey("dbo.LeaveStatus", t => t.LeaveStatusId, cascadeDelete: true)
                .ForeignKey("dbo.LeaveTypes", t => t.LeaveTypeId, cascadeDelete: true)
                .Index(t => t.LeaveTypeId)
                .Index(t => t.LeaveReasonId)
                .Index(t => t.LeaveStatusId);
            
            CreateTable(
                "dbo.LeaveReasons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveReasonText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LeaveStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveStatusText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LeaveTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveTypeText = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LeaveSessions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        YearId = c.Int(nullable: false),
                        SessionStartDate = c.DateTime(nullable: false),
                        SessionEndDate = c.DateTime(nullable: false),
                        SickLeaves = c.Int(nullable: false),
                        CasualLeaves = c.Int(nullable: false),
                        AnnualLeaves = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Loans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        LoanAllocatedDate = c.DateTime(nullable: false),
                        LoanTypeId = c.Int(nullable: false),
                        LoanAmount = c.Int(nullable: false),
                        InstallmentNumbers = c.Int(nullable: false),
                        InstallmentAmount = c.Int(nullable: false),
                        DeductableAmount = c.Int(nullable: false),
                        BalanceAmount = c.Int(nullable: false),
                        LoanStatusId = c.Int(nullable: false),
                        Remarks = c.String(),
                        AttachmentFilePath = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoanStatusTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoanStatusTypeText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoanTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoanTypeText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ManualGroupShiftAssigneds",
                c => new
                    {
                        ManualGroupShiftAssignedId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(),
                        reason = c.String(),
                        active = c.Boolean(nullable: false),
                        Employee_EmployeeId = c.Int(),
                        Group_GroupId = c.Int(),
                        Shift_ShiftId = c.Int(),
                    })
                .PrimaryKey(t => t.ManualGroupShiftAssignedId)
                .ForeignKey("dbo.Employees", t => t.Employee_EmployeeId)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId)
                .ForeignKey("dbo.Shifts", t => t.Shift_ShiftId)
                .Index(t => t.Employee_EmployeeId)
                .Index(t => t.Group_GroupId)
                .Index(t => t.Shift_ShiftId);
            
            CreateTable(
                "dbo.PaymentModeTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentModeText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentStatusTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentStatusText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payrolls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        SalaryMonthYear = c.DateTime(nullable: false),
                        CNIC = c.String(),
                        NTNNumber = c.String(),
                        EOBINumber = c.String(),
                        SESSINumber = c.String(),
                        BasicPay = c.Int(nullable: false),
                        Increment = c.Int(nullable: false),
                        Transport = c.Int(nullable: false),
                        Mobile = c.Int(nullable: false),
                        Medical = c.Int(nullable: false),
                        Food = c.Int(nullable: false),
                        Night = c.Int(nullable: false),
                        GroupAllowance = c.Int(nullable: false),
                        Commission = c.Int(nullable: false),
                        Rent = c.Int(nullable: false),
                        CashAllowance = c.Int(nullable: false),
                        AnnualBonus = c.Int(nullable: false),
                        LeavesCount = c.Int(nullable: false),
                        LeavesEncash = c.Int(nullable: false),
                        OvertimeInHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OvertimeAmount = c.Int(nullable: false),
                        IncomeTax = c.Int(nullable: false),
                        FineExtraAmount = c.Int(nullable: false),
                        MobileDeduction = c.Int(nullable: false),
                        AbsentsCount = c.Int(nullable: false),
                        AbsentsAmount = c.Int(nullable: false),
                        EOBIEmployee = c.Int(nullable: false),
                        EOBIEmployer = c.Int(nullable: false),
                        SESSIEmployee = c.Int(nullable: false),
                        SESSIEmployer = c.Int(nullable: false),
                        LoanInstallment = c.Int(nullable: false),
                        OtherDeduction = c.Int(nullable: false),
                        GrossSalary = c.Int(nullable: false),
                        TotalDeduction = c.Int(nullable: false),
                        NetSalary = c.Int(nullable: false),
                        PaymentModeId = c.Int(nullable: false),
                        BankNameId = c.Int(nullable: false),
                        BankAccTitle = c.String(),
                        BankAccNo = c.String(),
                        AckStatusId = c.Int(nullable: false),
                        PayStatusId = c.Int(nullable: false),
                        UserHRComments = c.String(),
                        AttachmentFilePath = c.String(),
                        IsFirstMonthText = c.String(),
                        PaymentDateTime = c.DateTime(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PayrollAmounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DesignationId = c.Int(nullable: false),
                        GradeId = c.Int(nullable: false),
                        BasicPay = c.Int(nullable: false),
                        Increment = c.Int(nullable: false),
                        Transport = c.Int(nullable: false),
                        Mobile = c.Int(nullable: false),
                        Medical = c.Int(nullable: false),
                        CashAllowance = c.Int(nullable: false),
                        Commission = c.Int(nullable: false),
                        Food = c.Int(nullable: false),
                        Night = c.Int(nullable: false),
                        Rent = c.Int(nullable: false),
                        GroupAllowance = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        UpdateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShiftToGroupRegisters",
                c => new
                    {
                        ShiftToGroupRegisterId = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        group_GroupId = c.Int(),
                        shift_ShiftId = c.Int(),
                    })
                .PrimaryKey(t => t.ShiftToGroupRegisterId)
                .ForeignKey("dbo.Groups", t => t.group_GroupId)
                .ForeignKey("dbo.Shifts", t => t.shift_ShiftId)
                .Index(t => t.group_GroupId)
                .Index(t => t.shift_ShiftId);
            
            CreateTable(
                "dbo.SkillsSets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        skillname = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SLMs",
                c => new
                    {
                        SLMId = c.Int(nullable: false, identity: true),
                        active = c.Boolean(nullable: false),
                        superLineManager_EmployeeId = c.Int(),
                        taggedEmployee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.SLMId)
                .ForeignKey("dbo.Employees", t => t.superLineManager_EmployeeId)
                .ForeignKey("dbo.Employees", t => t.taggedEmployee_EmployeeId)
                .Index(t => t.superLineManager_EmployeeId)
                .Index(t => t.taggedEmployee_EmployeeId);
            
            CreateTable(
                "dbo.Terminals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        L_ID = c.Int(nullable: false),
                        C_Name = c.String(),
                        terminal_id = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShiftGroups",
                c => new
                    {
                        Shift_ShiftId = c.Int(nullable: false),
                        Group_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Shift_ShiftId, t.Group_GroupId })
                .ForeignKey("dbo.Shifts", t => t.Shift_ShiftId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId, cascadeDelete: true)
                .Index(t => t.Shift_ShiftId)
                .Index(t => t.Group_GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SLMs", "taggedEmployee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.SLMs", "superLineManager_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ShiftToGroupRegisters", "shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.ShiftToGroupRegisters", "group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.ManualGroupShiftAssigneds", "Shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.ManualGroupShiftAssigneds", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.ManualGroupShiftAssigneds", "Employee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.LeaveApplications", "LeaveTypeId", "dbo.LeaveTypes");
            DropForeignKey("dbo.LeaveApplications", "LeaveStatusId", "dbo.LeaveStatus");
            DropForeignKey("dbo.LeaveApplications", "LeaveReasonId", "dbo.LeaveReasons");
            DropForeignKey("dbo.GeneralCalendars", "Shift6_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift5_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift4_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift3_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift2_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift1_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "Shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendars", "generalShift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GeneralCalendarOverrides", "GeneralCalendar_GeneralCalendarId", "dbo.GeneralCalendars");
            DropForeignKey("dbo.GeneralCalendarOverrides", "Shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.CS_PersistentLog", "employee_ContractualStaffId", "dbo.ContractualStaffs");
            DropForeignKey("dbo.CS_AttendanceLog", "CS_AttendanceLogId", "dbo.ContractualStaffs");
            DropForeignKey("dbo.ConsolidatedAttendancesArchives", "employee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ManualAttendances", "employee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.ManualAttendances", "ConsolidatedAttendance_ConsolidatedAttendanceId", "dbo.ConsolidatedAttendances");
            DropForeignKey("dbo.ConsolidatedAttendances", "employee_EmployeeId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "type_of_employment_TypeOfEmploymentId", "dbo.TypeOfEmployments");
            DropForeignKey("dbo.Employees", "region_RegionId", "dbo.Regions");
            DropForeignKey("dbo.PersistentAttendanceLogs", "PersistentAttendanceLogId", "dbo.Employees");
            DropForeignKey("dbo.Employees", "location_LocationId", "dbo.Locations");
            DropForeignKey("dbo.GroupCalendars", "wednesday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "tuesday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "thursday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "sunday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "saturday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "monday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupCalendars", "generalShift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendars", "friday_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendarOverrides", "Shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.ShiftGroups", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.ShiftGroups", "Shift_ShiftId", "dbo.Shifts");
            DropForeignKey("dbo.GroupCalendarOverrides", "GroupCalendar_GroupCalendarId", "dbo.GroupCalendars");
            DropForeignKey("dbo.Employees", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.Employees", "grade_GradeId", "dbo.Grades");
            DropForeignKey("dbo.Employees", "function_FunctionId", "dbo.Functions");
            DropForeignKey("dbo.Employees", "designation_DesignationId", "dbo.Designations");
            DropForeignKey("dbo.Employees", "department_DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Employees", "access_group_AccessGroupId", "dbo.AccessGroups");
            DropForeignKey("dbo.AttendanceRemarksRegisters", "Remark_RemarkId", "dbo.Remarks");
            DropIndex("dbo.ShiftGroups", new[] { "Group_GroupId" });
            DropIndex("dbo.ShiftGroups", new[] { "Shift_ShiftId" });
            DropIndex("dbo.SLMs", new[] { "taggedEmployee_EmployeeId" });
            DropIndex("dbo.SLMs", new[] { "superLineManager_EmployeeId" });
            DropIndex("dbo.ShiftToGroupRegisters", new[] { "shift_ShiftId" });
            DropIndex("dbo.ShiftToGroupRegisters", new[] { "group_GroupId" });
            DropIndex("dbo.ManualGroupShiftAssigneds", new[] { "Shift_ShiftId" });
            DropIndex("dbo.ManualGroupShiftAssigneds", new[] { "Group_GroupId" });
            DropIndex("dbo.ManualGroupShiftAssigneds", new[] { "Employee_EmployeeId" });
            DropIndex("dbo.LeaveApplications", new[] { "LeaveStatusId" });
            DropIndex("dbo.LeaveApplications", new[] { "LeaveReasonId" });
            DropIndex("dbo.LeaveApplications", new[] { "LeaveTypeId" });
            DropIndex("dbo.GeneralCalendarOverrides", new[] { "GeneralCalendar_GeneralCalendarId" });
            DropIndex("dbo.GeneralCalendarOverrides", new[] { "Shift_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift6_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift5_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift4_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift3_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift2_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift1_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "Shift_ShiftId" });
            DropIndex("dbo.GeneralCalendars", new[] { "generalShift_ShiftId" });
            DropIndex("dbo.CS_PersistentLog", new[] { "employee_ContractualStaffId" });
            DropIndex("dbo.CS_AttendanceLog", new[] { "CS_AttendanceLogId" });
            DropIndex("dbo.ContractualStaffs", new[] { "employee_code" });
            DropIndex("dbo.ConsolidatedAttendancesArchives", new[] { "employee_EmployeeId" });
            DropIndex("dbo.ManualAttendances", new[] { "employee_EmployeeId" });
            DropIndex("dbo.ManualAttendances", new[] { "ConsolidatedAttendance_ConsolidatedAttendanceId" });
            DropIndex("dbo.PersistentAttendanceLogs", new[] { "PersistentAttendanceLogId" });
            DropIndex("dbo.GroupCalendarOverrides", new[] { "Shift_ShiftId" });
            DropIndex("dbo.GroupCalendarOverrides", new[] { "GroupCalendar_GroupCalendarId" });
            DropIndex("dbo.GroupCalendars", new[] { "wednesday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "tuesday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "thursday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "sunday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "saturday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "monday_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "Group_GroupId" });
            DropIndex("dbo.GroupCalendars", new[] { "generalShift_ShiftId" });
            DropIndex("dbo.GroupCalendars", new[] { "friday_ShiftId" });
            DropIndex("dbo.Employees", new[] { "type_of_employment_TypeOfEmploymentId" });
            DropIndex("dbo.Employees", new[] { "region_RegionId" });
            DropIndex("dbo.Employees", new[] { "location_LocationId" });
            DropIndex("dbo.Employees", new[] { "Group_GroupId" });
            DropIndex("dbo.Employees", new[] { "grade_GradeId" });
            DropIndex("dbo.Employees", new[] { "function_FunctionId" });
            DropIndex("dbo.Employees", new[] { "designation_DesignationId" });
            DropIndex("dbo.Employees", new[] { "department_DepartmentId" });
            DropIndex("dbo.Employees", new[] { "access_group_AccessGroupId" });
            DropIndex("dbo.Employees", new[] { "employee_code" });
            DropIndex("dbo.ConsolidatedAttendances", new[] { "employee_EmployeeId" });
            DropIndex("dbo.AttendanceRemarksRegisters", new[] { "Remark_RemarkId" });
            DropTable("dbo.ShiftGroups");
            DropTable("dbo.Terminals");
            DropTable("dbo.SLMs");
            DropTable("dbo.SkillsSets");
            DropTable("dbo.ShiftToGroupRegisters");
            DropTable("dbo.PayrollAmounts");
            DropTable("dbo.Payrolls");
            DropTable("dbo.PaymentStatusTypes");
            DropTable("dbo.PaymentModeTypes");
            DropTable("dbo.ManualGroupShiftAssigneds");
            DropTable("dbo.LoanTypes");
            DropTable("dbo.LoanStatusTypes");
            DropTable("dbo.Loans");
            DropTable("dbo.LeaveSessions");
            DropTable("dbo.LeaveTypes");
            DropTable("dbo.LeaveStatus");
            DropTable("dbo.LeaveReasons");
            DropTable("dbo.LeaveApplications");
            DropTable("dbo.HaTransits");
            DropTable("dbo.GeneralCalendarOverrides");
            DropTable("dbo.GeneralCalendars");
            DropTable("dbo.FutureManualAttendances");
            DropTable("dbo.EmployeeForgotPasswords");
            DropTable("dbo.Letters");
            DropTable("dbo.EmployeeEvaluations");
            DropTable("dbo.CS_PersistentLog");
            DropTable("dbo.CS_AttendanceLog");
            DropTable("dbo.ContractualStaffs");
            DropTable("dbo.ConsolidatedAttendancesArchives");
            DropTable("dbo.ManualAttendances");
            DropTable("dbo.TypeOfEmployments");
            DropTable("dbo.Regions");
            DropTable("dbo.PersistentAttendanceLogs");
            DropTable("dbo.Locations");
            DropTable("dbo.Shifts");
            DropTable("dbo.GroupCalendarOverrides");
            DropTable("dbo.GroupCalendars");
            DropTable("dbo.Groups");
            DropTable("dbo.Grades");
            DropTable("dbo.Functions");
            DropTable("dbo.Designations");
            DropTable("dbo.Departments");
            DropTable("dbo.Employees");
            DropTable("dbo.ConsolidatedAttendances");
            DropTable("dbo.BankNames");
            DropTable("dbo.AuditTrails");
            DropTable("dbo.Remarks");
            DropTable("dbo.AttendanceRemarksRegisters");
            DropTable("dbo.AccessGroups");
            DropTable("dbo.AccessCodeValues");
        }
    }
}
