// var mp3Path = "E:\\Music\\Input\\aaa.mp3";
// var outputPath = "E:\\Music\\Input\\bbbb.mp3";
// WavFileUtils.TrimMp3File(mp3Path, outputPath, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(8));
// return;

var curDir = Environment.CurrentDirectory;
var rootPath = curDir.Split("AudioClib\\bin")[0];
var loadPath = "";
string inputPath = "Input";
string outPath = "Export";
string inputFullPath = rootPath + inputPath;
string exportFullPath = rootPath + outPath;
if (!Directory.Exists(inputFullPath))
{
    Directory.CreateDirectory(inputFullPath);
}

if (!Directory.Exists(exportFullPath))
{
    Directory.CreateDirectory(exportFullPath);
}

var GetData = (string filePath) =>
{
    string[] lines = System.IO.File.ReadAllLines(filePath);
    List<AudioData> dataList = new List<AudioData>();
    for (int i = 0; i < lines.Length; i++)
    {
        // 最后一个
        if (i == lines.Length - 1)
        {
            dataList.Add(new AudioData(lines[i],""));
        }
        else
        {
            dataList.Add(new AudioData(lines[i],lines[i+1]));
        }
    }

    return dataList;
};



var files = Directory.GetFiles(inputFullPath);
foreach (var fileInfo in files)
{
    FileInfo info = new FileInfo(fileInfo);
    if (info.Extension == (".txt"))
    {
        var fileName = info.Name.Split('.')[0];
        var audioFile = fileInfo.Split('.')[0] + ".wav";
        var mp3File = fileInfo.Split('.')[0] + ".mp3";
        var targetAudioFile = "";
        if (File.Exists(audioFile))
        {
            targetAudioFile = audioFile;
        }
        else if (File.Exists(mp3File))
        {
            targetAudioFile = mp3File;
        }
        else
        {
            Console.WriteLine("没有找到音频文件：" + fileName);
            continue;
        }

        var singerPath = exportFullPath + "\\" + fileName;
        Console.WriteLine("开始剪切音频：" + singerPath);
        if (!Directory.Exists(singerPath))
        {
            Directory.CreateDirectory(singerPath);
        }

        // string jsonStr = File.ReadAllText(fileInfo);
        // var dataList = JsonConvert.DeserializeObject<List<AudioData>>(jsonStr);
        var dataList = GetData(fileInfo);
        if (dataList != null) WavFileUtils.TrimWavListFile(targetAudioFile, singerPath, dataList);
        // if (dataList != null) WavFileUtils.TrimMp3ListFile(targetAudioFile, singerPath, dataList);
        
        
    }
}


public class AudioData
{
    public string Name { get; set; }
    public int StartTime { get; set; }
    public int EndTime { get; set; }

    public AudioData(string name, int startTime, int endTime)
    {
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
    }

    private int GetTime(string data)
    {
        var timeStrArray = data.Split(" ",2);
        var timeStr = timeStrArray[0];
        var startTimeArray = timeStr.Split(':');
        if (startTimeArray.Length == 3) {
            return int.Parse(startTimeArray[0]) * 3600 + int.Parse(startTimeArray[1]) * 60 + int.Parse(startTimeArray[2]);
        }
        return int.Parse(startTimeArray[0])* 60 + int.Parse(startTimeArray[1]);
    }
    
    public AudioData(string curData,string nextStr)
    {
        var str = curData.Replace(" - ", " -");
        str = str.Replace("  ", " ");
        str = str.Replace("?", "");
        str = str.Replace("-", "");
        var strList = str.Split(" ",2);
        var timeStr = strList[0];
        Name = strList[1];
        StartTime = GetTime(curData);
        if (string.IsNullOrEmpty(nextStr))
        {
            EndTime = -1;
        }
        else
        {
            EndTime = GetTime(nextStr);
        }
        
    }
}