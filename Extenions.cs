public static class Extenions
{

    public static Template.Language LanguageFromString(this string language) => language switch
    {
        "C#" => Template.Language.CSharp,
        "F#" => Template.Language.FSharp,
        "VB" => Template.Language.VB,
        "" => Template.Language.None,
        _ => throw new ArgumentException($"Unknown language: {language}")
    };

    public static Template.TemplateType TemplateTypeFromString(this string? templateType) => templateType switch
    {
        "project" => Template.TemplateType.Project,
        "item" => Template.TemplateType.Item,
        "" => Template.TemplateType.Other,
        _ => throw new ArgumentException($"Unknown template type: {templateType}")
    };

    public static string LanguageToString(this Template.Language language) => language switch
    {
        Template.Language.CSharp => "C#",
        Template.Language.FSharp => "F#",
        Template.Language.VB => "VB",
        Template.Language.None => "",
        _ => throw new ArgumentException($"Unknown language: {language}")
    };

    public static string LanguageToProjFile(this Template.Language language) => language switch
    {
        Template.Language.CSharp => "csproj",
        Template.Language.FSharp => "fsproj",
        Template.Language.VB => "vbproj",
        Template.Language.None => "",
        _ => throw new ArgumentException($"Unknown language: {language}")
    };



}
