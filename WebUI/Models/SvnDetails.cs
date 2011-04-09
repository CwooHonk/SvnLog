using System.Collections.Generic;
using SvnDomainModel;


public class SvnDetails
{
    public List<Svn.LogEntry> Changes { get; set; }
    public string TrunckPath {get; set;}
    public string BranchPath { get; set; }

    public SvnDetails()
    {
        Changes = new List<Svn.LogEntry>();
    }
}