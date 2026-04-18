namespace DLL_UNIS.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cAuthority")]
    public partial class cAuthority
    {
        [Key]
        [StringLength(4)]
        public string C_Code { get; set; }

        [StringLength(30)]
        public string C_Name { get; set; }

        public int? L_SetLocal { get; set; }

        public int? L_RegInfo { get; set; }

        public int? L_DataBackup { get; set; }

        public int? L_MgrTerminal { get; set; }

        public int? L_RegControl { get; set; }

        public int? L_SetControl { get; set; }

        public int? L_RegEmploye { get; set; }

        public int? L_ModEmploye { get; set; }

        public int? L_OutEmploye { get; set; }

        public int? L_RegVisitor { get; set; }

        public int? L_OutVisitor { get; set; }

        public int? L_RegMoney { get; set; }

        public int? L_RegWork { get; set; }

        public int? L_SetWork { get; set; }

        public int? L_ModWork { get; set; }

        public int? L_RegMeal { get; set; }

        public int? L_SetMeal { get; set; }

        public int? L_ModMeal { get; set; }

        public int? L_DelResult { get; set; }

        public int? L_DelWork { get; set; }

        public int? L_DelMeal { get; set; }

        public int? L_MgrScope { get; set; }

        public int? L_ChgBlacklist { get; set; }

        public int? L_RelBlacklist { get; set; }

        public int? L_ModBlacklist { get; set; }

        public int? L_DelBlacklist { get; set; }

        public int? L_Customized { get; set; }

        public int? L_RegAdmin { get; set; }

        public int? L_ModAdmin { get; set; }

        public int? L_SetShutdown { get; set; }

        public int? L_MntMgr { get; set; }

        public int? L_MntClient { get; set; }

        public int? L_MntTerminal { get; set; }

        public int? L_MntAuthLog { get; set; }

        public int? L_MntEvent { get; set; }

        public int? L_TmnMgr { get; set; }

        public int? L_TmnAdd { get; set; }

        public int? L_TmnMod { get; set; }

        public int? L_TmnDel { get; set; }

        public int? L_TmnUpgrade { get; set; }

        public int? L_TmnOption { get; set; }

        public int? L_TmnAdmin { get; set; }

        public int? L_TmnSendFile { get; set; }

        public int? L_EmpMgr { get; set; }

        public int? L_EmpAdd { get; set; }

        public int? L_EmpMod { get; set; }

        public int? L_EmpDel { get; set; }

        public int? L_EmpSendTerminal { get; set; }

        public int? L_EmpTerminalMng { get; set; }

        public int? L_EmpRegAdmin { get; set; }

        public int? L_VstMgr { get; set; }

        public int? L_VstAdd { get; set; }

        public int? L_VstMod { get; set; }

        public int? L_VstDel { get; set; }

        public int? L_BlckMgr { get; set; }

        public int? L_BlckChange { get; set; }

        public int? L_BlckRelease { get; set; }

        public int? L_BlckDel { get; set; }

        public int? L_BlckMod { get; set; }

        public int? L_AccMgr { get; set; }

        public int? L_AccSet { get; set; }

        public int? L_MapMgr { get; set; }

        public int? L_MapSet { get; set; }

        public int? L_TnaMgr { get; set; }

        public int? L_TnaSet { get; set; }

        public int? L_TnaSpecial { get; set; }

        public int? L_TnaWork { get; set; }

        public int? L_TnaOutState { get; set; }

        public int? L_TnaOutExcRecord { get; set; }

        public int? L_TnaSummary { get; set; }

        public int? L_TnaSendResult { get; set; }

        public int? L_TnaDelData { get; set; }

        public int? L_MealMgr { get; set; }

        public int? L_MealOutRecord { get; set; }

        public int? L_MealDelData { get; set; }

        public int? L_MealOutDept { get; set; }

        public int? L_MealOutPerson { get; set; }

        public int? L_MealSet { get; set; }

        public int? L_LogMgr { get; set; }

        public int? L_LogOutRecord { get; set; }

        public int? L_LogDelRecord { get; set; }

        public int? L_SetRegInfo { get; set; }

        public int? L_SetMgr { get; set; }

        public int? L_SetServer { get; set; }

        public int? L_SetPwd { get; set; }

        public int? L_SetMail { get; set; }

        public int? L_SetEtc { get; set; }
    }
}
