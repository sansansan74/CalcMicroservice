using System;
using System.Collections.Generic;
using System.Linq;

namespace CalcMicroservice.Services;

public class ParamsStarterService
{
    enum ModeEnum
    {
        Include,
        Exclude
    }

    private const string StartModulesName = "STARTMODULES";
    List<string> ModulesList = [];
    ModeEnum Mode = ModeEnum.Exclude;


    public ParamsStarterService()
    {
    }
    public void Init(string[] args)
    {
        string startModules = ReadStartModulesParams(args);

        var modules = startModules
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToUpper())
            .Distinct()
            .ToList();

        var modulesPlus = modules.Where(x => x.StartsWith('+')).ToList();
        var modulesMunus = modules.Where(x => x.StartsWith('-')).ToList();

        if (modulesMunus.Count > 0 && modulesPlus.Count > 0)
            throw new ArgumentException("You can use only + or only -. You can not mis + and - params");

        if (modulesMunus.Count > 0)
        {
            Mode = ModeEnum.Exclude;
            ModulesList = modulesMunus.Select(x => x.Replace("-", "")).ToList();
        }

        if (modulesPlus.Count > 0)
        {
            Mode = ModeEnum.Include;
            ModulesList = modulesPlus.Select(x => x.Replace("+", "")).ToList();
        }
    }

    private static string ReadStartModulesParams(string[] args)
    {
        string fromArgs = ReadFromArgs(args);
        string fromEnv = Environment.GetEnvironmentVariable(StartModulesName) ?? string.Empty;

        if (!string.IsNullOrEmpty(fromArgs) && !string.IsNullOrEmpty(fromEnv))
            throw new ArgumentException($"You can set modules only in system variable {StartModulesName} or in argument {StartModulesName} in command list. But not both system variable and command argument.");

        if (!string.IsNullOrEmpty(fromArgs))
            return fromArgs;

        return Environment.GetEnvironmentVariable(StartModulesName) ?? string.Empty;
    }

    /// <summary>
    /// Find param with name STARTMODULES
    /// </summary>
    /// <param name="args">args from command line</param>
    /// <returns>value of parameter</returns>
    /// <exception cref="ArgumentException"></exception>
    private static string ReadFromArgs(string[] args)
    {
        var prefix = $"{StartModulesName}=";
        var arg = args.Where(x => x.StartsWith(prefix)).ToList();

        if (arg.Count == 0)
            return string.Empty;

        if (arg.Count > 1)
        {
            throw new ArgumentException("Program can have only 1 argument STARTMODULES");
        }


        // STARTMODULES="+MOD1 +MOD2"
        // STARTMODULES='+MOD1 +MOD2'
        // STARTMODULES=+MOD1
        var val = arg.Single()[prefix.Length..];
        val = val.Replace("'", "").Replace("\"", "").Trim();
        return val;
    }

    public bool ContainsModule(string module)
    {
        if (Mode == ModeEnum.Include)
            return ModulesList.Contains(module);

        if (Mode == ModeEnum.Exclude)
            return !ModulesList.Contains(module);
        // mode is explude
        throw new Exception("Mode can be only Include or Exclule");
    }

}
