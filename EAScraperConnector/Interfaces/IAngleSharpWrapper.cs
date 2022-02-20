using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using Microsoft.AspNetCore.Mvc;

namespace EAScraperConnector.Interfaces
{
    public interface IAngleSharpWrapper     
    {
        public Task<IDocument> GetSearchResults(string url, IRequester? request = null);

        public Task<IDocument> OpenAsync(string url, RequesterWrapper requester);
    }
}
