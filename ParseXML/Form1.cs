using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;

namespace ParseXML
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Количество операторов в одном экспортируемом файле.
        /// </summary>
        private const int ExportSize = 2000;

        /// <summary>
        /// Количество потоков (параллельно анализируемых файлов).
        /// </summary>
        private const int Parallelism = 1;

        /// <summary> Файл, который необходимо парсить </summary>
        private static readonly string FileXMLPath = @"C:\Users\SERGI\Desktop\OpenData";

        /// <summary>
        /// Номера анализируемых XML-файлов.
        /// </summary>
        int[] fileNumber = new int[Parallelism];

        private string[] files;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Основная форма приложения.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _ = ThreatGeneratorAsync();
        }

        /// <summary>
        /// Номер файла XML в files_txt.
        /// </summary>
        /// <param name="file_number">Номер XML-файла в папке.</param>
        private async Task Parser(int file_number)
        {
            try
            {
                await Task.Delay(1);
                //int f = file_number;
                // загрузка файла Открытых данных, ВНИМАНИЕ: файл более 3 Гб, загрузка занимает длительное время и может не хватить оперативной памяти !!!
                XmlDocument docXML = new XmlDocument(); // XML-документ
                try
                {
                    docXML.Load(files[file_number]); // загрузить XML
                }
                catch { }
                string[] export_lines = new string[ExportSize]; // Строки, что будут записаны в экспортируемый файл.
                string[] files_txt_f = files[file_number].Split('\\');
                string folder = Path.Combine(FileXMLPath, Path.GetFileNameWithoutExtension(files[file_number]));
                string outputstring = Path.Combine(folder, "Export-" + Path.GetFileNameWithoutExtension(files[file_number]) + ".csv"); // Путь к экспортируемому файлу
                string outputstring_new = outputstring.Split('.')[0];
                Directory.CreateDirectory(folder);

                int export_lines_count = 0;
                Stopwatch stopWatch = new Stopwatch();
                for (int i = 0; i < ExportSize && i < docXML.ChildNodes[1].ChildNodes.Count; i++)
                {
                    stopWatch = new Stopwatch();
                    stopWatch.Start();
                    string purpose_txt = docXML.GetElementsByTagName("rkn:purpose_txt")[i].InnerText;
                    string basis = docXML.GetElementsByTagName("rkn:basis")[i].InnerText;
                    string resp_name = string.Empty;
                    try
                    {
                        resp_name = docXML.GetElementsByTagName("rkn:resp_name")[i].InnerText;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString()); // Тут могут быть ошибки, полезно их увидеть
                    }

                    string[] result = new string[10];

                    result[0] = docXML.GetElementsByTagName("rkn:pd_operator_num")[i].InnerText;
                    result[1] = docXML.GetElementsByTagName("rkn:name_full")[i].InnerText;
                    result[2] = docXML.GetElementsByTagName("rkn:inn")[i].InnerText;
                    result[3] = docXML.GetElementsByTagName("rkn:address")[i].InnerText;
                    result[4] = docXML.GetElementsByTagName("rkn:income_date")[i].InnerText;

                    if (purpose_txt.Contains("\r"))
                    {
                        purpose_txt = purpose_txt.Replace('\r', ';');
                        purpose_txt = purpose_txt.Replace('\n', ' ');
                    }

                    result[5] = purpose_txt;

                    if (basis.Contains("\r"))
                    {
                        basis = basis.Replace('\r', ';');
                        basis = basis.Replace('\n', ';');
                    }
                    result[6] = basis;
                    result[7] = docXML.GetElementsByTagName("rkn:enter_order_date")[i].InnerText;
                    result[8] = resp_name;

                    var ddd = docXML.ChildNodes[1].ChildNodes[i]["rkn:is_list"]; // вытаскиваем список ИСПДн компании
                    try
                    {
                        if (ddd.ChildNodes.Count > 0)
                        {
                            List<ISPDn> iSPDns = new List<ISPDn>();
                            string isdnns_string = string.Empty;
                            for (int k = 0; k < ddd.ChildNodes.Count; k++)
                            {
                                iSPDns.Add(
                                    new ISPDn(
                                        ddd.GetElementsByTagName("rkn:pd_category")[k].InnerText,
                                        ddd.GetElementsByTagName("rkn:category_sub_txt")[k].InnerText,
                                        ddd.GetElementsByTagName("rkn:actions_category")[k].InnerText,
                                        ddd.GetElementsByTagName("rkn:pd_handle")[k].InnerText,
                                        ddd.GetElementsByTagName("rkn:transgran_transfer")[k].InnerText,
                                        ddd.GetElementsByTagName("rkn:db_country")[k].InnerText
                                        )
                                    );
                            }

                            for (int k = 0; k < iSPDns.Count; k++)
                            {
                                isdnns_string +=
                                    "ИСПДн " + k + ": " +
                                    "Категории Пдн: " + iSPDns[k].pd_category +// Категории персональных данных
                                    "Субъекты ПДн: " + iSPDns[k].category_sub_txt +// Категории субъектов, персональные данные которых обрабатываются
                                    "Действия: " + iSPDns[k].actions_category +// Перечень действий с персональными данными
                                    "Обработка: " + iSPDns[k].pd_handle +// Обработка персональных данных
                                    "Трансгр: " + iSPDns[k].transgran_transfer +// Наличие трансграничной передачи
                                    "Страна базы: " + iSPDns[k].db_country;// Сведения о местонахождении базы данных
                                if (k != (iSPDns.Count - 1))
                                {
                                    isdnns_string += "____";
                                }
                            }
                            isdnns_string = isdnns_string.Replace('\r', ';');
                            isdnns_string = isdnns_string.Replace('\n', ' ');

                            result[9] = isdnns_string;
                        }
                        else
                        {
                            result[9] = string.Empty;
                        }
                    }
                    catch
                    {
                        result[9] = string.Empty; // Иногда ИСПДн вообще нет :(
                        File.AppendAllText(FileXMLPath + @"\errors.txt", "Error: Нет ИСПДн у " + result[0] + Environment.NewLine);
                    }
                    export_lines[i] = string.Join("|", result);
                    export_lines_count++;

                    if ((export_lines_count > ExportSize - 1) || (export_lines_count == docXML.ChildNodes[1].ChildNodes.Count - 1))
                    {
                        try
                        {
                            for (int r = 0; r < ExportSize; r++)
                            {
                                if (docXML.ChildNodes[1].ChildNodes.Count > 0)
                                    docXML.ChildNodes[1].RemoveChild(docXML.ChildNodes[1].ChildNodes[0]); // удаляем 1000 первых нод, каждый раз удаляя первую
                                else break;
                            }

                            docXML.Save(files[file_number]); // Запись урезанного файла без первых XML-нод в количестве ExportSize

                            TimeSpan ts = stopWatch.Elapsed;
                            string time_string = " fileNumber=" + ts.Hours + "h " + ts.Minutes + "min " + ts.Seconds + "sec";

                            //File.WriteAllLines(outputstring_new + " №" + docXML.ChildNodes[1].ChildNodes.Count + time_string + ".csv", export_lines, Encoding.GetEncoding("windows-1251"));
                            File.WriteAllLines(
                                Path.Combine(
                                    outputstring_new,
                                    " №" + docXML.ChildNodes[1].ChildNodes.Count + time_string + ".csv"
                                    ),
                                export_lines,
                                Encoding.GetEncoding("windows-1251"));

                            // Очистка данных в конце цикла
                            stopWatch.Reset();
                            export_lines_count = 0;
                            export_lines = new string[ExportSize];
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString()); // Тут могут быть ошибки, полезно их увидеть
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString()); // Тут могут быть ошибки, полезно их увидеть
            }
        }
        /// <summary>
        /// Проверяет наличие совпадение текущего значения элемента fileNumber с иными 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool checkСoincidence(int n)
        {
            bool same = false;
            for (int i = 0; i < fileNumber.Length; i++)
            {
                if (n == fileNumber[i])
                {
                    same = true; break;
                }
            }
            return same;
        }

        private void UpdateFile()
        {
            bool isFilesEnd = true;
            try
            {
                for (int i = 0; i < Parallelism; i++)
                {
                    for (int ff = 0; ff < files.Length; ff++)
                    {
                        FileInfo fi = new FileInfo(files[ff]);
                        if (fi.Length > 256)
                        {
                            isFilesEnd = false;
                            if (!checkСoincidence(ff))
                            {
                                fileNumber[i] = ff; break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString()); // Тут могут быть ошибки, полезно их увидеть
            }
            if (isFilesEnd)
            {
                exit = true;
            }
        }
        /// <summary>
        /// Завершать ли экспорт из XML.
        /// </summary>
        bool exit = false;
        /// <summary>
        /// Основная функция парсинга XML-файлов.
        /// </summary>
        /// <returns>Ничего не возвращает.</returns>
        private async Task ThreatGeneratorAsync()
        {
            try
            {
                files = Directory.GetFiles(FileXMLPath, "*.xml", SearchOption.AllDirectories);
                if (files.Length > 0)
                {

                    for (int i = 0; i < Parallelism; i++)
                    {
                        fileNumber[i] = i;
                    }
                    UpdateFile();

                    do
                    {
                        List<Task> tasks = new List<Task>(Parallelism) { };
                        //for (int i = 0; i < Parallelism; i++)
                        {
                            tasks.Add(Task.Run(() => Parser(fileNumber[0]))); // генерируем потоки
                            //tasks.Add(Task.Run(() => Parser(fileNumber[1]))); // генерируем потоки
                        }

                        // Обновляем список файлов на форме
                        label2.Text = string.Empty;
                        for (int i = 0; i < Parallelism; i++)
                        {
                            label2.Text += files[fileNumber[i]] + Environment.NewLine;
                        }

                        await Task.WhenAll(tasks.AsParallel().Select(async task => await task)); // запуск парсинга

                        UpdateFile();
                        tasks.Clear();
                    }
                    while (exit == false); // костыль                    

                    // Собираем большой CSV из мелких
                    label2.Text = "Подготовка итогового файла";
                    string[] BigOpenDataBlocksPaths = Directory.GetFiles(FileXMLPath, "*.csv", SearchOption.AllDirectories);
                    string BigOpenDataFullPath = FileXMLPath + @"\BigOpenData.csv";
                    List<string> BigOpenDataStringsFromBlocks = new List<string>();
                    for (int i = 0; i < BigOpenDataBlocksPaths.Length; i++)
                    {
                        if (!Path.GetFileName(BigOpenDataBlocksPaths[i]).Equals("BigOpenData.csv"))
                        {
                            BigOpenDataStringsFromBlocks.AddRange(File.ReadAllLines(BigOpenDataBlocksPaths[i], Encoding.GetEncoding("windows-1251")));
                        }
                        else { }
                    }

                    // Сохраняем большой CSV
                    // Создание отчетного файла и первой строки
                    label2.Text = "Сохранение итогового файла";
                    string[] first_line = new string[10];
                    first_line[0] = "Номер Оператора";
                    first_line[1] = "Наименование";
                    first_line[2] = "ИНН";
                    first_line[3] = "Адрес";
                    first_line[4] = "Субъекты РФ";
                    first_line[5] = "Цель обработки";
                    first_line[6] = "Основание обработки";
                    first_line[7] = "Дата внесения в реестр";
                    first_line[8] = "Ответственный за ПДн";
                    first_line[9] = "Реестр ИСПДн";
                    string result_string = string.Join("|", first_line);
                    File.WriteAllLines(BigOpenDataFullPath, BigOpenDataStringsFromBlocks, Encoding.GetEncoding("windows-1251"));
                    File.AppendAllLines(BigOpenDataFullPath, BigOpenDataStringsFromBlocks, Encoding.GetEncoding("windows-1251"));

                    File.Open(BigOpenDataFullPath, FileMode.Open);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString()); // Тут могут быть ошибки, полезно их увидеть
            }
        }
    }
}