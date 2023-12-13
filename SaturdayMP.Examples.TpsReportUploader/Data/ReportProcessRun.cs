namespace SaturdayMP.Examples.TpsReportUploader.Data;

public class ReportProcessRun
{
    public int ID { get; set; }
    public DateTime RunDate { get; set; }
    
    public List<ReportProcessRunItem> ReportProcessRunItems { get; set; }
}