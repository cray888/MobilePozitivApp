using System;
using System.Collections.Generic;

namespace MobilePozitivApp
{
    public class News
    {
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Autor { get; set; }
        public bool Fix { get; set; }
    }

    public class DataNews
    {
        static List<News> mNews;

        public DataNews()
        {
            mNews = new List<News>();
        }

        public void Add(News news)
        {
            mNews.Add(news);
        }

        public int NumNews
        {
            get { return mNews.Count; }
        }

        public News this[int i]
        {
            get { return mNews[i]; }
        }
    }
}