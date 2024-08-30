using FormulaParser.Tree;
using System.Threading.Tasks;

namespace CalcMicroservice.Services
{
    public interface ITreePersistantStorageService
    {
        public Task Set(ICalcTreeItem calcTree, string value);
        public Task Set(string calcTree, string value);
        public Task<string> Get(ICalcTreeItem calcTree);
        public Task<string> Get(string calcTree);
    }
}