using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using CommandAssemble;
namespace CommandAssemble
{
    public interface ICommand
    {
        string Name { get; }
        List<string> secondKeyWord { get; }
        CommandRunResult Run(string[] args);
    }
    public struct CommandRunResult
    {
        public CommandRunResult(bool isCorrect, int? errorCode = null, string errorMessage = null, string resultMessage = "")
        {
            this.isCorrect = isCorrect;
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.resultMessage = resultMessage;
        }
        public readonly string resultMessage;
        public readonly bool isCorrect;
        public readonly int? errorCode;
        public readonly string errorMessage;
    }
    class Lua : ICommand
    {
        public string Name { get; } = "Lua";
        public List<string> secondKeyWord { get; } = new List<string>() { "mode" };
        public CommandRunResult Run(string[] args)
        {
            if (secondKeyWord.Contains(args[1]))
            {

            }
            else
            {
                return new CommandRunResult(false, 1, $"{Name} 内没有 {args[1]} 这个二级函数");
            }
            return new CommandRunResult(true, null, "");
        }
    }
    class Help : ICommand
    {
        public string Name { get; } = "help";
        public List<string> secondKeyWord { get; } = new List<string>() { null };
        public CommandRunResult Run(string[] args)
        {
            return new CommandRunResult(true, resultMessage: @"light [bool] : 是否全局接受2D光照
tilemap speed [int] : 设置生成多少方格更新一帧
tilemap collider [bool] : 设置生成方格时是否取消碰撞体碰撞
seed get : 获取当前随机数种子
seed set [int] : 设置新种子
seed random : 随机生成新种子
");
        }

    }
    class Update : MonoBehaviour,ICommand
    {
        public string Name { get; } = "help";
        public List<string> secondKeyWord { get; } = new List<string>() { null };
        public CommandRunResult Run(string[] args)
        {
            CheckUpdate checkUpdateScript = ((GameObject)Command.s_commandData[0]).GetComponent<CheckUpdate>();
            StartCoroutine(checkUpdateScript.DownloadApplicationFile($@"http://d0.ananas.chaoxing.com/download/{args[1]}"));
            return new CommandRunResult();
        }
    }
    class BaiduApi : ICommand
    {
        public string Name { get; } = "baiduapi";
        public List<string> secondKeyWord { get; } = new List<string>() { null };
        public CommandRunResult Run(string[] args)
        {
            RunC(args);
            return new CommandRunResult();
        }
        public void RunC(string[] args)
        {
            byte[] b=System.IO.File.ReadAllBytes(args[1]);
            string base64 = System.Convert.ToBase64String(b);
            System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://ai.baidu.com/aidemo");
            httpWebRequest.AllowAutoRedirect = false;
            httpWebRequest.Method = "POST";
            
            var h = new System.Net.WebHeaderCollection() {
                ["Cookie"]= "BIDUPSID=FF85CD68CD87CFCC847E78714FC662A8; PSTM=1594707137; BAIDUID=FF85CD68CD87CFCC9C556C125AABC6B6:FG=1; BD_UPN=12314753; __yjs_duid=1_70971348300143b033ad6a69e18b04c01608804495086; BDORZ=B490B5EBF6F3CD402E515D22BCDA1598; BD_HOME=1; BDRCVFR[feWj1Vr5u3D]=I67x6TjHwwYf0; delPer=0; BD_CK_SAM=1; PSINO=2; BAIDUID_BFESS=0A049245A92284FCA4F8FAD647607807:FG=1; ab_sr=1.0.0_ZDE0MjlmOGMwMGNmYzQxN2Y3YjY3MGM5YjRkZDAyOTVhZmJkOWI4ZjAwMjBiNWM3ZjZkYTNmNGZkOGI0OGU0NmFiYjk5YWQyZWEzMDBlODc4Yzg2ZGJjNDRiNzMwYjEy; __yjsv5_shitong=1.0_7_1164b3d892c3dd7869c1f77e8a33f32bdd31_300_1610588770834_121.19.208.220_4815a7d7; H_PS_PSSID=33425_33471_33404_33257_33344_31254_33286_26350_33394_33370; H_PS_645EC=e9cajaf8jHqHNaw15g6Xonr7lLTQ%2BHFAk461yDhBaPFKoDOcO79%2FEnHas8xeNngQGn1T; BDUSS=Gp0RE9XTVE3N09BRjN4ZzMwOHB0RHBXU3ByLTN0UFhKR3RWQlBRNlN5TmdPaWRnRVFBQUFBJCQAAAAAAAAAAAEAAAC-opbKwt7AssCyxrEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGCt~19grf9fT; BDUSS_BFESS=Gp0RE9XTVE3N09BRjN4ZzMwOHB0RHBXU3ByLTN0UFhKR3RWQlBRNlN5TmdPaWRnRVFBQUFBJCQAAAAAAAAAAAEAAAC-opbKwt7AssCyxrEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGCt~19grf9fT; BA_HECTOR=a5850ha50g2k01815s1fvvbj60r"
            };
            
            httpWebRequest.Headers = h;
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Referer = "https://ai.baidu.com/tech/imageprocess/style_trans?qq-pf-to=pcqq.group";
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36 Edg/87.0.664.75";
            
            using (System.IO.Stream r = httpWebRequest.GetRequestStream())
            {
                var a = System.Text.Encoding.ASCII.GetBytes($"image=data%3Aimage%2Fjpeg%3Bbase64%2C{base64}&image_url=&type=https%3A%2F%2Faip.baidubce.com%2Frest%2F2.0%2Fimage-process%2Fv1%2Fstyle_trans&option=cartoon");
                r.Write(a,0,a.Length);
            }
            using  (System.Net.HttpWebResponse r = (System.Net.HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (System.IO.StreamReader rea = new System.IO.StreamReader(r.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    string re = rea.ReadToEnd();
                    LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(re);
                    byte[] buffers = System.Convert.FromBase64String(jsonData["data"]["image"].ToString());
                    System.IO.File.WriteAllBytes(@"D:/wuhu.jpg", buffers);
                }
            }
        }
    }
    class Light : ICommand
    {
        public string Name { get; } = "light";
        public List<string> secondKeyWord { get; } = new List<string>();
        public CommandRunResult Run(string[] args)
        {
            var material = ((GameObject)Command.s_commandData[0]).GetComponent<TilemapRenderer>();
            if (args[1] == "true")
            {
                material.material = (Material)Command.s_commandData[1];
            }
            else if (args[1] == "false")
            {
                material.material = (Material)Command.s_commandData[2];
            }
            else
            {
                return new CommandRunResult(false, 10, "只能是true或者false");
            }
            return new CommandRunResult(true);
        }
    }
    class Seed : ICommand
    {
        public string Name { get; } = "seed";
        public List<string> secondKeyWord { get; } = new List<string>() { "set", "get", "random" };
        public CommandRunResult Run(string[] args)
        {
            switch (args[1])
            {
                case "get":
                    {
                        return new CommandRunResult(true, resultMessage: RandomSeedPlugin.NowSeed.ToString());
                    }
                case "set":
                    {
                        if (int.TryParse(args[2], out int result))
                        {
                            result.SetSeed();
                            return new CommandRunResult(true);
                        }
                        else { return new CommandRunResult(false, 11, $"{args[2]} 不是数字"); }
                    }
                case "random":
                    {
                        int seed = Random.Range(10000, 10000000).SetSeed();
                        return new CommandRunResult(true, resultMessage: $"{seed} 是新地图种子");
                    }
                default: return new CommandRunResult(false, 1, $"{Name} 内没有 ${args[2]} 这个二级函数");
            }
        }
    }
    class TilemapCommand : ICommand
    {
        public string Name { get; } = "tilemap";
        public List<string> secondKeyWord { get; } = new List<string>() { "speed", "collider" };
        public CommandRunResult Run(string[] args)
        {
            TilemapSpawn tilemapSpawn = ((GameObject)Command.s_commandData[3]).GetComponent<TilemapSpawn>();
            switch (args[1])
            {
                case "speed":
                    {
                        if (int.TryParse(args[2], out int result))
                        {
                            tilemapSpawn.spawnMapSpeed = result;
                            return new CommandRunResult(true);
                        }
                        else
                        {
                            return new CommandRunResult(false, 11, $"{args[2]} 不是数字");
                        }
                    }
                case "collider":
                    {
                        if (args[2] == "true") { tilemapSpawn.isUpdateColliderOnFrame = true; return new CommandRunResult(true); }
                        else if (args[2] == "false") { tilemapSpawn.isUpdateColliderOnFrame = false; return new CommandRunResult(true); }
                        else
                        {
                            return new CommandRunResult(false, 10, "只能是true或者false");
                        }
                    }
                default:
                    {
                        return new CommandRunResult(false, 1, $"{Name} 内没有 {args[1]} 这个二级函数");
                    }
            }

        }
    }
}
public class Command : MonoBehaviour
{
    public static Dictionary<int, Object> commandDictData = new Dictionary<int, Object>();
    public List<Object> commandData = new List<Object>();
    public static List<Object> s_commandData = new List<Object>();
    public static Dictionary<string, ICommand> commandKeyWord = new Dictionary<string, ICommand>() {
        ["lua"] = new Lua(),
        ["light"] = new CommandAssemble.Light(),
        ["tilemap"] = new TilemapCommand(),
        ["help"]=new Help(),
        ["seed"]=new Seed(),
        ["update"]=new Update(),
        ["baiduapi"]=new BaiduApi()
    };
    private void Awake()
    {
        s_commandData = commandData;
    }
    public static CommandRunResult RunCommand(string command)
    {
        string[] args = command.Split(' ');
        if (commandKeyWord.ContainsKey(args[0]))
        {
            return commandKeyWord[args[0]].Run(command.Split(' '));
        }
        else
        {
            return new CommandRunResult(false, 0, "没有找到函数");
        }
    }
}
