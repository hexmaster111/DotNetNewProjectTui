using static Template;

public static class DotNetTemplateParcer
{

    public static IEnumerable<Template> Parce(
        string outputString, out Template.Language defaultLanguage
        )
    {
        const string colSeparator = "  ";
        const int colSeparatorLength = 2;
        const string tagsSeparator = "/";
        const string shortNamesSeparator = ",";
        const string languageSeparator = ",";

        int templateNameLength = 0;
        int templateShortNameLength = 0;
        int templateLanguageLength = 0;
        int templateTypeLength = 0;
        int templateAuthorLength = 0;
        int templateTagsLength = 0;

        var outLines = outputString.Split(Environment.NewLine);
        var colSeparatorLineSplit = outLines[3].Split(colSeparator);
        int linesAfterColSeparator = outLines.Length - 4;

        templateNameLength = colSeparatorLineSplit[(int)TemplateColumns.Name].Length;
        templateShortNameLength = colSeparatorLineSplit[(int)TemplateColumns.ShortName].Length;
        templateLanguageLength = colSeparatorLineSplit[(int)TemplateColumns.Language].Length;
        templateTypeLength = colSeparatorLineSplit[(int)TemplateColumns.Type].Length;
        templateAuthorLength = colSeparatorLineSplit[(int)TemplateColumns.Author].Length;
        templateTagsLength = colSeparatorLineSplit[(int)TemplateColumns.Tags].Length;

        //Default language is the language with the [] around it
        defaultLanguage = outLines[4]
            .Substring(templateNameLength + templateShortNameLength + colSeparatorLength * 2, templateLanguageLength)
            .Trim()
            .Split("[")
            .Last()
            .Split("]")
            .First()
            .LanguageFromString();

        var templates = new List<Template>();
        for (int line = 4; line < linesAfterColSeparator + 2; line++)
        {
            var lengthOffset = 0;

            var templateName = outLines[line]
                .Substring(0, templateNameLength)
                .Trim();
            lengthOffset += templateNameLength + colSeparatorLength;

            var templateShortNames = outLines[line]
                .Substring(lengthOffset, templateShortNameLength)
                .Trim()
                .Split(shortNamesSeparator);
            lengthOffset += templateShortNameLength + colSeparatorLength;


            var templateLanguages = outLines[line]
                .Substring(lengthOffset, templateLanguageLength)
                .Trim()
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Split(languageSeparator)
                .Select(x => x.LanguageFromString());
            lengthOffset += templateLanguageLength + colSeparatorLength;

            var templateType = outLines[line]
                .Substring(lengthOffset, templateTypeLength)
                .Trim()
                .ToLower()
                .TemplateTypeFromString();

            lengthOffset += templateTypeLength + colSeparatorLength;

            var templateAuthor = outLines[line]
                .Substring(lengthOffset, templateAuthorLength)
                .Trim();
            lengthOffset += templateAuthorLength + colSeparatorLength;

            var templateTags = outLines[line]
                .Substring(lengthOffset, templateTagsLength)
                .Trim()
                .Split(tagsSeparator);

            templates.Add(Template.New(
                templateName,
                templateShortNames,
                templateAuthor,
                templateLanguages,
                templateType,
                templateTags
            ));
        }
        return templates;
    }

    public static string[] GetProjectedChanges(
        Template template,
        string solutionName,
        string projectName,
        string solutionDirectory,
        Language language
    )
    {
        var baseFolderPath = Path.Combine(solutionDirectory, solutionName);
        var slnPath = Path.Combine(baseFolderPath, $"{solutionName}.sln");
        var projectPath = Path.Combine(baseFolderPath, projectName);

        var projFileExtention = language.LanguageToProjFile();
        var projectFilePath = Path.Combine(projectPath, $"{projectName}.{projFileExtention}");

        return new string[]
        {
            slnPath,
            projectPath,
            projectFilePath
        };
    }




    enum TemplateColumns
    {
        Name,
        ShortName,
        Language,
        Type,
        Author,
        Tags
    }

}