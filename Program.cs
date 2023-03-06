using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;
using Terminal.Gui;
using static Template;

const string dotnetFileLocationLinux = "/usr/share/dotnet/dotnet";
const string listAllDotnetTemplatesArguments = "new list --columns-all";

var resault = await Cli.Wrap(dotnetFileLocationLinux)
    .WithArguments(listAllDotnetTemplatesArguments)
    .ExecuteBufferedAsync();

var templates = DotNetTemplateParcer.Parce(resault.StandardOutput, out var language);


//Steps for making a new project with a sln file and a project in its own folder in the sln
// dotnet new sln --output SolutionName --name SolutionName
// cd SolutionName
// dotnet new [TEMPLATE] --output ProjectName --name ProjectName --language [LANGUAGE]
// dotnet sln add ProjectName/ProjectName.csproj


Application.Init();
Application.Run(new NewSolutionDialog.NewSolutionDialog(templates, language, BuildProject));
Application.Shutdown();



void BuildProject(
        Template template,
        string solutionName,
        string projectName,
        string solutionDirectory,
        Language language
    )
{
    var sln = Cli.Wrap(dotnetFileLocationLinux)
        .WithArguments($"new sln --output {solutionName} --name {solutionName}");
    var project = Cli.Wrap(dotnetFileLocationLinux)
        .WithArguments(
            $"new {template.Name} --output {projectName} --name {projectName} --language {language.ToString().ToLower()}"
        );
    var slnAdd = Cli.Wrap(dotnetFileLocationLinux)
        .WithArguments($"sln add {projectName}/{projectName}.csproj");

    var slnProcess = sln.ExecuteAsync();
    var projectProcess = project.ExecuteAsync();
    var slnAddProcess = slnAdd.ExecuteAsync();

    Task.WaitAll(slnProcess, projectProcess, slnAddProcess);

    var slnProcessResult = slnProcess.Task.Result;
    var projectProcessResult = projectProcess.Task.Result;
    var slnAddProcessResult = slnAddProcess.Task.Result;

}

public readonly struct Template
{
    public string Name { get; init; }
    public IEnumerable<string> ShortNames { get; init; }
    public string Author { get; init; }
    public IEnumerable<Language> LanguagesSupported { get; init; }
    public TemplateType Type { get; init; }
    public IEnumerable<string> Tags { get; init; }
    public static Template New
    (
        string name,
        IEnumerable<string> shortNames,
        string author,
        IEnumerable<Language> languagesSupported,
        TemplateType type,
        IEnumerable<string> tags
    )
    {
        return new Template
        {
            Name = name,
            ShortNames = shortNames,
            Author = author,
            LanguagesSupported = languagesSupported,
            Type = type,
            Tags = tags
        };
    }

    public override string ToString()
    {
        return Name;
    }

    public enum Language
    {
        CSharp,
        FSharp,
        VB,
        None
    }

    public enum TemplateType
    {
        Project,
        Item,
        Other
    }
}
