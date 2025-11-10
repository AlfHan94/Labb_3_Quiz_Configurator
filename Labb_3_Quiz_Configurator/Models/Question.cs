namespace Labb_3_Quiz_Configurator.Models;

public class Question
{
    public Question() { }
    public Question(string query, string correctAnswer,
        string incorrectAnswer1, string incorrectAnswer2, string incorrectAnswer3)
    {
        Query = query;
        CorrectAnswer = correctAnswer;
        IncorrectAnswers = new List<string>()
        {
            incorrectAnswer1, incorrectAnswer2, incorrectAnswer3
        };
    }
    public string Query { get; set; }
    public string CorrectAnswer { get; set; }
    public List<string> IncorrectAnswers { get; set; } = new();
}
