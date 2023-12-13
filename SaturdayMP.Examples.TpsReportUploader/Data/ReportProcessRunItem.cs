namespace SaturdayMP.Examples.TpsReportUploader.Data;

public class ReportProcessRunItem
{
    public int ID { get; set; }
    public string ReportName { get; set; }
    
    public ReportProcessRun ReportProcessRun { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}