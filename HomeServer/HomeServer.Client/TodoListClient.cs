using System;
using System.Runtime.CompilerServices;

namespace HomeServer.Client;

public class TodoListClient
{
    /// <summary>
    /// The Todo List client.
    /// </summary>
    public IClient TodoList => _todoClient;
    private Client _todoClient { get; }

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
        }
    }
    private string _baseUrl;

    /// <summary>
    /// Constructs the Todo List client for use within primarily the web app project.
    /// </summary>
    public TodoListClient(HttpClient httpClient, string baseUrl)
    {
        _todoClient = new Client(baseUrl, httpClient);
        _baseUrl = baseUrl;
    }
}
