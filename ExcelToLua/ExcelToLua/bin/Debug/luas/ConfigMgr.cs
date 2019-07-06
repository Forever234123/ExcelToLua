using System.Collections;
using System.Threading;
using UnityEngine;

public delegate byte[] LoadBytesDelegate(string assetpoath);
public class ConfigMgr
{
    public static TestMgr  Test;
public static Test2Mgr  Test2;


    public static string Path;
    public static LoadBytesDelegate LoadBytes;
    public static void Init(string path, LoadBytesDelegate customload)
    {
	Path = path;
        	LoadBytes = customload;
        	ThreadPool.SetMaxThreads(5, 5);
    }

    public static void ReadData()
    {
        Test= new TestMgr();
ThreadPool.QueueUserWorkItem(Test.ReadConfig);
Test2= new Test2Mgr();
ThreadPool.QueueUserWorkItem(Test2.ReadConfig);

    }
}
