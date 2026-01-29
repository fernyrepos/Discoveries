using System.Collections.Generic;
using Verse;
namespace Discoveries
{
    public class UnlockResearchOnDiscovery : DefModExtension
    {
        public string researchProject;
        public List<string> researchProjects;
        public IEnumerable<ResearchProjectDef> GetProjects()
        {
            if (researchProject.NullOrEmpty() is false)
            {
                var def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(researchProject);
                if (def != null)
                {
                    yield return def;
                }
            }
            if (researchProjects != null)
            {
                foreach (var proj in researchProjects)
                {
                    var def = DefDatabase<ResearchProjectDef>.GetNamedSilentFail(proj);
                    if (def != null)
                    {
                        yield return def;
                    }
                }
            }
        }
    }
}
