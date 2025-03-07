﻿using BlogWebApp.Models;
using X.PagedList;



namespace BlogWebApp.ViewModels
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ShortName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public IPagedList<Post>? Posts { get; set; }

    }
}
