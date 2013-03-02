using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace towerDefence
{
    public class TwitterStream
    {
        private readonly string username;
        private readonly string password;
        string stream_url = ConfigurationManager.AppSettings["stream_url"];
        HttpWebRequest webRequest = null;
        HttpWebResponse webResponse = null;
        StreamReader responseStream = null;
        DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Status));
        public delegate void TweetHandler(Status s);

        private TweetHandler handler;

        public TwitterStream(string username, string password, TweetHandler tweetHandler)
        {
            this.username = username;
            this.password = password;
            this.handler = tweetHandler;
        }

        public void Stream(string[] tags)
        {
            string tagList = "";
            foreach (var tag in tags)
            {
                tagList += tag + ",";
            }
            string postparameters = ("&track=" + tagList);


            int wait = 250;
            string jsonText = "";

            try
            {
                while (true)
                {
                    try
                    {
                        //Connect
                        webRequest = (HttpWebRequest)WebRequest.Create(stream_url);
                        webRequest.Credentials = new NetworkCredential(username, password);
                        webRequest.Timeout = -1;

                        Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                        if (postparameters.Length > 0)
                        {
                            webRequest.Method = "POST";
                            webRequest.ContentType = "application/x-www-form-urlencoded";

                            byte[] _twitterTrack = encode.GetBytes(postparameters);

                            webRequest.ContentLength = _twitterTrack.Length;
                            Stream _twitterPost = webRequest.GetRequestStream();
                            _twitterPost.Write(_twitterTrack, 0, _twitterTrack.Length);
                            _twitterPost.Close();
                        }

                        webResponse = (HttpWebResponse)webRequest.GetResponse();
                        responseStream = new StreamReader(webResponse.GetResponseStream(), encode);

                        //Read the stream.
                        while (true)
                        {
                            jsonText = responseStream.ReadLine();

                            byte[] byteArray = Encoding.UTF8.GetBytes(jsonText);
                            MemoryStream stream = new MemoryStream(byteArray);

                            //TODO:  Check for multiple objects.
                            var status = json.ReadObject(stream) as Status;
                            //Success
                            wait = 250;

                            //Write Status
                            handler(status);

                        }
                        //Abort is needed or responseStream.Close() will hang.
                        webRequest.Abort();
                        responseStream.Close();
                        responseStream = null;
                        webResponse.Close();
                        webResponse = null;

                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex.Message);
                        //logger..append(ex.Message, Logger.LogLevel.ERROR);
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            //-- From Twitter Docs -- 
                            //When a HTTP error (> 200) is returned, back off exponentially. 
                            //Perhaps start with a 10 second wait, double on each subsequent failure, 
                            //and finally cap the wait at 240 seconds. 
                            //Exponential Backoff
                            if (wait < 10000)
                            {
                                wait = 10000;
                            }
                            else
                            {
                                if (wait < 240000)
                                {
                                    wait = wait * 2;
                                }
                            }
                        }
                        else
                        {
                            //-- From Twitter Docs -- 
                            //When a network error (TCP/IP level) is encountered, back off linearly. 
                            //Perhaps start at 250 milliseconds and cap at 16 seconds.
                            //Linear Backoff
                            if (wait < 16000)
                            {
                                wait += 250;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        
                    }
                    finally
                    {
                        if (webRequest != null)
                        {
                            webRequest.Abort();
                        }
                        if (responseStream != null)
                        {
                            responseStream.Close();
                            responseStream = null;
                        }

                        if (webResponse != null)
                        {
                            webResponse.Close();
                            webResponse = null;
                        }
                        Console.WriteLine("Waiting: " + wait);
                        Thread.Sleep(wait);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //logger.append(ex.Message, Logger.LogLevel.ERROR);
                Console.WriteLine("Waiting: " + wait);
                Thread.Sleep(wait);
            }
        }


       
    }
}
