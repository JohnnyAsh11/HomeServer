using System;
using System.Runtime.CompilerServices;

namespace HomeServer.Client;

public class HomeServerClient
{
    /// <summary>
    /// The Todo List client.
    /// </summary>
    public ITaskClient TodoList => _todoClient;
    private TaskClient _todoClient { get; }

    /// <summary>
    /// The Ollama AI chat client.
    /// </summary>
    public IChatClient Chat => _chatClient;
    private ChatClient _chatClient { get; }

    /// <summary>
    /// The base url of the api.
    /// </summary>
    public string BaseUrl
    {
        get { return _baseUrl; }
        set
        {
            _baseUrl = value;

            // Ensuring that the base url ends with the needed slash.
            if (!string.IsNullOrEmpty(_baseUrl) && !_baseUrl.EndsWith('/'))
            {
                _baseUrl += '/';
            }

            _todoClient.BaseUrl = _baseUrl;
            _chatClient.BaseUrl = _baseUrl;
        }
    }
    private string _baseUrl;

    /// <summary>
    /// Constructs the Todo List client for use within primarily the web app project.
    /// </summary>
    public HomeServerClient(HttpClient httpClient, string baseUrl)
    {
        _todoClient = new TaskClient(baseUrl, httpClient);
        _chatClient = new ChatClient(baseUrl, httpClient);
        _baseUrl = baseUrl;
    }
}
