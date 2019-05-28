using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SP_TEST
{
    class CDataToolTest
    {
    }
    class CMyFileHandler
    {
        // Constructor
        CMyFileHandler()
        {

        }
        #region [ GetMatchedFiles] : 주어진 Directory에 하위 모든 폴더에서 주어진 이름을 가진 파일을 찾아 경로를 리턴해줌
        public static int FindMatchedFilesFromSubDirectory(string path, string file_name, out string[] result)
        {
            // find all files which is matched given file_name
            result = Directory.GetFiles(path, file_name, SearchOption.AllDirectories);
            return result.Length;
        }
        #endregion

        #region [ copy file ] 특정 파일을 원하는 위치에 카피함
        public static void CopyFile(string file_name)
        {
            System.IO.Directory.CreateDirectory(".\\OUTPUT");
            const int BUF_SIZE = 512;

            byte[] buffer = new byte[BUF_SIZE];
            int nFReadLen;
            using (FileStream fs_in = new FileStream("./INPUT/" + file_name, FileMode.Open, FileAccess.Read),
                 fs_out = new FileStream("./OUTPUT/" + file_name, FileMode.Create, FileAccess.Write))
            {
                while ((nFReadLen = fs_in.Read(buffer, 0, BUF_SIZE)) > 0)
                {
                    fs_out.Write(buffer, 0, nFReadLen);
                }
            }
        }
        #endregion
        #region [ StreamReader ]
        public static bool ReadFile(string file_name, ref List<string> result)
        {
            if (File.Exists(file_name))
            {
                // using문을 사용하면 Diposal를 자동 처리 즉 file close를 알아서 처리해줌
                using (StreamReader reader = new StreamReader(file_name))
                {
                    while (true)
                    {
                        string line = reader.ReadLine();
                        if (line == null)
                            return true;
                        result.Add(line);
                    }
                }
            }
            return false;
        }
        #endregion

        #region[ StreamWrite ]
        public static bool WriteFile(string out_file_name, List<string> data)
        {
            bool result = true;

            using (var writer = new StreamWriter(out_file_name, true, Encoding.UTF8))
            {
                foreach (string line in data)
                {
                    writer.WriteLine(line);
                }
            }

            return result;
        }
        #endregion
    }

    class CMyFileTest
    {
        static void runTest(string[] args)
        {
            #region [ GetMatchedFiles 예제]
            string[] result;
            int num = CMyFileHandler.FindMatchedFilesFromSubDirectory(@"D:\imdb", "clrpick.tcl", out result);
            foreach (string s in result)
            {
                Console.WriteLine(s);
            }
            #endregion
            #region[ CopyFile 예제 ]
            CMyFileHandler.CopyFile("test.exe");
            #endregion
            #region[ readFile ]
            string readFileName = @"..\..\data\program.txt";
            List<string> datalist = new List<string>();
            bool success = CMyFileHandler.ReadFile(readFileName, ref datalist);
            if (success)
            {
                foreach (string line in datalist)
                {
                    Console.WriteLine(line);
                }
            }
            #endregion
            #region [StreamWrite]
            string writeFileName = @"..\..\data\program2.txt";
            success = CMyFileHandler.WriteFile(writeFileName, datalist);
            #endregion
            Console.ReadKey();
        }
    }
    // String 처리 관련 참고 사항 Class
    class CMyString
    {
        #region [ string : IndexOf, LastIndexOf, StartsWith 및 EndsWith 메서드를 사용하여 문자열을 검색]
        public static void HandleStringExample1()
        {
            string str = "Extension methods have all the capabilities of regular static methods.";

            // Write the string and include the quotation marks.
            Console.WriteLine("\"{0}\"", str);

            // Simple comparisons are always case sensitive! 
            bool test1 = str.StartsWith("extension");
            Console.WriteLine("Starts with \"extension\"? {0}", test1);

            // For user input and strings that will be displayed to the end user,  
            // use the StringComparison parameter on methods that have it to specify how to match strings. 
            bool test2 = str.StartsWith("extension", StringComparison.CurrentCultureIgnoreCase);
            Console.WriteLine("Starts with \"extension\"? {0} (ignoring case)", test2);

            bool test3 = str.EndsWith(".", StringComparison.CurrentCultureIgnoreCase);
            Console.WriteLine("Ends with '.'? {0}", test3);

            // This search returns the substring between two strings, so  
            // the first index is moved to the character just after the first string. 
            int first = str.IndexOf("methods") + "methods".Length;
            int last = str.LastIndexOf("methods");
            string str2 = str.Substring(first, last - first);
            Console.WriteLine("Substring between \"methods\" and \"methods\": '{0}'", str2);


        }
        #endregion
        #region[ string : Split 사용 ]
        public static void string_split_test()
        {
            //1.문자열을 공백을 기준으로 단어로 분리
            {
                string phrase = "The quick brown fox jumped over the lazy dog.";
                string[] words = phrase.Split(' ');

                foreach (var word in words)
                {
                    System.Console.WriteLine($"<{word}>");
                }
            }
            //2. 연속된 구분 문자는 반환된 배열의 값으로 빈 문자열을 생성
            {
                string phrase = "The quick brown    fox     jumped over the lazy dog.";
                string[] words = phrase.Split(' ');

                foreach (var word in words)
                {
                    System.Console.WriteLine($"<{word}>");
                }
            }
            //3.다중 구분 문자를 사용
            {
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

                string text = "one\ttwo three:four,five six seven";
                System.Console.WriteLine($"Original text: '{text}'");

                string[] words = text.Split(delimiterChars);
                System.Console.WriteLine($"{words.Length} words in text:");

                foreach (var word in words)
                {
                    System.Console.WriteLine($"<{word}>");
                }
            }
            //4.연속되는 모든 구분 기호의 인스턴스는 출력 배열에 빈 문자열을 생성
            {
                char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

                string text = "one\ttwo :,five six seven";
                System.Console.WriteLine($"Original text: '{text}'");

                string[] words = text.Split(delimiterChars);
                System.Console.WriteLine($"{words.Length} words in text:");

                foreach (var word in words)
                {
                    System.Console.WriteLine($"<{word}>");
                }
            }
            //5.문자열 배열(단일 문자 대신 대상 문자열을 구문 분석하는 구분 기호 역할을 
            //하는 문자 시퀀스)을 사용
            {
                string[] separatingChars = { "<<", "..." };

                string text = "one<<two......three<four";
                System.Console.WriteLine("Original text: '{0}'", text);

                string[] words = text.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
                System.Console.WriteLine("{0} substrings in text:", words.Length);

                foreach (var word in words)
                {
                    System.Console.WriteLine(word);
                }
            }
        }
        #endregion
        #region[ string : formatting ]
        public static void print_formmated_string_test()
        {
            Int32 positiveNum = 123456;
            Int32 negativeNum = -123456;
            Single realNumber32 = 123456.789F;
            Double realNumber64 = 123456.789012345;
            String text = "String.Format example";

            Console.WriteLine("서식이 지정되지 않은 그대로의 값: ");
            Console.WriteLine("  positiveNum : {0}", positiveNum);
            Console.WriteLine("  negativeNum : {0}", negativeNum);
            Console.WriteLine("  realNumber32: {0}", realNumber32);
            Console.WriteLine("  realNumber64: {0}", realNumber64);
            Console.WriteLine("  text        : {0}", text);

            Console.WriteLine("통화 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:c}", positiveNum);
            Console.WriteLine("  negativeNum : {0:C}", negativeNum);
            Console.WriteLine("  realNumber32: {0:c}", realNumber32);
            Console.WriteLine("  realNumber64: {0:C}", realNumber64);
            Console.WriteLine("  text        : {0:C}", text);

            Console.WriteLine("10진수 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:d}", positiveNum);
            Console.WriteLine("  negativeNum : {0:D}", negativeNum);
            Console.WriteLine("  realNumber32: -");
            Console.WriteLine("  realNumber64: -");
            Console.WriteLine("  text        : {0:D}", text);

            Console.WriteLine("고정 소숫점 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:f}", positiveNum);
            Console.WriteLine("  negativeNum : {0:F}", negativeNum);
            Console.WriteLine("  realNumber32: {0:f}", realNumber32);
            Console.WriteLine("  realNumber64: {0:F}", realNumber64);
            Console.WriteLine("  text        : {0:F}", text);

            Console.WriteLine("일반 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:g}", positiveNum);
            Console.WriteLine("  negativeNum : {0:G}", negativeNum);
            Console.WriteLine("  realNumber32: {0:g}", realNumber32);
            Console.WriteLine("  realNumber64: {0:G}", realNumber64);
            Console.WriteLine("  text        : {0:G}", text);

            Console.WriteLine("숫자 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:n}", positiveNum);
            Console.WriteLine("  negativeNum : {0:N}", negativeNum);
            Console.WriteLine("  realNumber32: {0:n}", realNumber32);
            Console.WriteLine("  realNumber64: {0:N}", realNumber64);
            Console.WriteLine("  text        : {0:N}", text);

            Console.WriteLine("백분율 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:p}", positiveNum);
            Console.WriteLine("  negativeNum : {0:P}", negativeNum);
            Console.WriteLine("  realNumber32: {0:p}", realNumber32);
            Console.WriteLine("  realNumber64: {0:P}", realNumber64);
            Console.WriteLine("  text        : {0:P}", text);

            Console.WriteLine("라운드트립 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : -");
            Console.WriteLine("  negativeNum : -");
            Console.WriteLine("  realNumber32: {0:r}", realNumber32);
            Console.WriteLine("  realNumber64: {0:R}", realNumber64);
            Console.WriteLine("  text        : {0:R}", text);

            Console.WriteLine("16진수 서식이 지정된 값: ");
            Console.WriteLine("  positiveNum : {0:x}", positiveNum);
            Console.WriteLine("  negativeNum : {0:X}", negativeNum);
            Console.WriteLine("  realNumber32: -");
            Console.WriteLine("  realNumber64: -");
            Console.WriteLine("  text        : {0:X}", text);
        }
        #endregion

        #region[ string : between ]
        public static String between_strings_test(String text, String start, String end)
        {
            int p1 = text.IndexOf(start) + start.Length;
            int p2 = text.IndexOf(end, p1);
            if (end == "") return (text.Substring(p1));
            else
            {
                return text.Substring(p1, p2 - p1);
            }
        }
        #endregion

        #region [ string : 정규식 숫자 추출 ]
        public static string get_decimal_from_str_test(string input, string rep)
        {
            // \d : 숫자 \D: 문자를 의미
            // 따라서 하기 내용은 문자열중에서 숫자를 제외한 문자만 골라서 rep 문자로 대체한다.
            string strNum = Regex.Replace(input, @"\D", rep);
            return strNum;
        }
        #endregion

        public static void Test_SplitStringIntoStringArray(bool doTest)
        {
            if (!doTest) return;
            string org = "abcdefghijklm";
            char[] arr = org.ToCharArray();
            foreach (char a in arr)
            {
                string aa = a.ToString();
                Console.WriteLine(a);//내부적으로 ToString이 호출됨
                Console.WriteLine(aa);
            }
        }

        #region [ TEST ]
        public static void DoTest(bool doTest)
        {
            if (doTest)
            {

                HandleStringExample1();
                string_split_test();
                print_formmated_string_test();

                #region [ example of between ]
                {
                    String text = "One=1,Two=2,ThreeFour=34";

                    Console.WriteLine(between_strings_test(text, "One=", ",")); // 1
                    Console.WriteLine(between_strings_test(text, "Two=", ",")); // 2
                    Console.WriteLine(between_strings_test(text, "ThreeFour=", "")); // 34
                }
                #endregion

                #region [ example of GetDecimal ]
                {
                    string strDel = get_decimal_from_str_test("123adkdi67dkdkdkdk7d88888a", " ");
                    //string[] words = strDel.Split(' ');
                    var words = strDel.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    Console.WriteLine("input words :{0} and # of splited words:{1}", strDel, words.Length);

                    foreach (string word in words)
                    {
                        Console.Write("{0} ", word);
                    }
                    Console.WriteLine();

                }
                #endregion

                Test_SplitStringIntoStringArray(true);
            }
        }
        #endregion

    }
    class CMyNativeArray
    {
        public static void simpleArray()
        {
            int[] numbers = { 1, 2, 3, 4, 5 };
            int lengthOfNumbers = numbers.Length;
            Console.WriteLine(lengthOfNumbers);
        }
        // Rank 속성을 사용하여 배열의 차원 수를 표시
        public static void GetDimensionOfArray()
        {
            int[,] theArray = new int[5, 10];
            Console.WriteLine("the array has {0} dimensions.", theArray.Rank);
        }

        //multiple array 사용법
        public static void MultipleArrayUsage()
        {
            // Two-dimensional array.
            int[,] array2D = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            // The same array with dimensions specified.
            int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
            // A similar array with string elements.
            string[,] array2Db = new string[3, 2] { { "one", "two" }, { "three", "four" },
                                        { "five", "six" } };

            // Three-dimensional array.
            int[,,] array3D = new int[,,] { { { 1, 2, 3 }, { 4, 5, 6 } },
                                 { { 7, 8, 9 }, { 10, 11, 12 } } };
            // The same array with dimensions specified.
            int[,,] array3Da = new int[2, 2, 3] { { { 1, 2, 3 }, { 4, 5, 6 } },
                                       { { 7, 8, 9 }, { 10, 11, 12 } } };

            // Accessing array elements.
            Console.WriteLine(array2D[0, 0]);
            Console.WriteLine(array2D[0, 1]);
            Console.WriteLine(array2D[1, 0]);
            Console.WriteLine(array2D[1, 1]);
            Console.WriteLine(array2D[3, 0]);
            Console.WriteLine(array2Db[1, 0]);
            Console.WriteLine(array3Da[1, 0, 1]);
            Console.WriteLine(array3D[1, 1, 2]);

            // Getting the total count of elements or the length of a given dimension.
            var allLength = array3D.Length;
            var total = 1;
            for (int i = 0; i < array3D.Rank; i++)
            {
                total *= array3D.GetLength(i);
            }
            Console.WriteLine("{0} equals {1}", allLength, total);

            //차수를 지정하지 않고 배열을 초기화
            int[,] array4 = { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };

            //초기화하지 않고 배열 변수를 선언하도록 선택할 경우 new 연산자를 사용하여 변수에 배열을 할당해야 함
            int[,] array5;
            array5 = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };   // OK
                                                                              //array5 = {{1,2}, {3,4}, {5,6}, {7,8}};   // Error

            // 특정 배열 요소에 값을 할당
            array5[2, 1] = 25;

            // 특정 배열 요소의 값을 가져와 elementValue 변수에 할당
            int elementValue = array5[2, 1];

            //  가변 배열을 제외하고 배열 요소를 기본 값으로 초기화
            int[,] array6 = new int[10, 10];

        }
        // foreach 사용
        public static void foreachUsageWithArray()
        {
            int[] numbers = { 4, 5, 6, 1, 2, 3, -2, -1, 0 };
            foreach (int i in numbers)
            {
                Console.Write("{0} ", i);
            }
            // Output: 4 5 6 1 2 3 -2 -1 0

            int[,] numbers2D = new int[3, 2] { { 9, 99 }, { 3, 33 }, { 5, 55 } };
            // Or use the short form:
            // int[,] numbers2D = { { 9, 99 }, { 3, 33 }, { 5, 55 } };

            foreach (int i in numbers2D)
            {
                Console.Write("{0} ", i);
            }
            // Output: 9 99 3 33 5 55
            Console.WriteLine();

        }
        // 배열을 인수로 전달
        #region [ 배열을 인수로 전달 ]
        static void PrintArray(string[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i] + "{0}", i < arr.Length - 1 ? " " : "");
            }
            Console.WriteLine();
        }

        static void ChangeArray(string[] arr)
        {
            // The following attempt to reverse the array does not persist when
            // the method returns, because arr is a value parameter.
            arr = (arr.Reverse()).ToArray();
            // The following statement displays Sat as the first element in the array.
            Console.WriteLine("arr[0] is {0} in ChangeArray.", arr[0]);
        }

        static void ChangeArrayElements(string[] arr)
        {
            // The following assignments change the value of individual array 
            // elements. 
            arr[0] = "Sat";
            arr[1] = "Fri";
            arr[2] = "Thu";
            // The following statement again displays Sat as the first element
            // in the array arr, inside the called method.
            Console.WriteLine("arr[0] is {0} in ChangeArrayElements.", arr[0]);
        }
        static void Test_PassArrayAsArguments()
        {
            // Declare and initialize an array.
            string[] weekDays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            // Pass the array as an argument to PrintArray.
            PrintArray(weekDays);

            // ChangeArray tries to change the array by assigning something new
            // to the array in the method. 
            ChangeArray(weekDays);

            // Print the array again, to verify that it has not been changed.
            Console.WriteLine("Array weekDays after the call to ChangeArray:");
            PrintArray(weekDays);
            Console.WriteLine();

            // ChangeArrayElements assigns new values to individual array
            // elements.
            ChangeArrayElements(weekDays);

            // The changes to individual elements persist after the method returns.
            // Print the array, to verify that it has been changed.
            Console.WriteLine("Array weekDays after the call to ChangeArrayElements:");
            PrintArray(weekDays);
        }
        #endregion

        #region [ ref , out and array ]
        //out array
        static void FillArrayWithOut(out int[] arr)
        {
            arr = new int[5] { 1, 2, 3, 4, 5 };
        }
        static void Test_FillArrayWithOut()
        {
            int[] theArray; // Initialization is not required

            // Pass the array to the callee using out:
            FillArrayWithOut(out theArray);

            // Display the array elements:
            System.Console.WriteLine("Array elements are:");
            for (int i = 0; i < theArray.Length; i++)
            {
                Console.Write(theArray[i] + " ");
            }
            Console.WriteLine();
        }
        //ref array
        static void FillArrayWithRef(ref int[] arr)
        {
            if (arr == null)
            {
                arr = new int[10];
            }
            arr[0] = 1111;
            arr[4] = 5555;
        }
        static void Test_FillArrayWithRef()
        {
            // Initialize the array:
            int[] theArray = { 1, 2, 3, 4, 5 };

            // Pass the array using ref:
            FillArrayWithRef(ref theArray);

            // Display the updated array:
            System.Console.WriteLine("Array elements are:");
            for (int i = 0; i < theArray.Length; i++)
            {
                Console.Write(theArray[i] + " ");
            }
            Console.WriteLine();
        }
        #endregion

        #region [ TEST ]
        public static void DoTest(bool doTest)
        {
            if (doTest)
            {
                simpleArray();
                GetDimensionOfArray();
                MultipleArrayUsage();
                foreachUsageWithArray();
                Test_PassArrayAsArguments();
                Test_FillArrayWithOut();
                Test_FillArrayWithRef();

            }
        }
        #endregion
    }
    public class Department
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class DepartmentComparer : IEqualityComparer<Department>
    {
        // equal if their Codes are equal
        public bool Equals(Department x, Department y)
        {
            // reference the same objects?
            if (Object.ReferenceEquals(x, y)) return true;

            // is either null?
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Code == y.Code;
        }

        public int GetHashCode(Department dept)
        {
            // If Equals() returns true for a pair of objects 
            // then GetHashCode() must return the same value for these objects.

            // if null default to 0
            if (Object.ReferenceEquals(dept, null)) return 0;

            return dept.Code.GetHashCode();
        }
    }
    class CMyList2
    {
        static private Department[] departments = { new Department { Code = "MK", Name = "Marketing" },
                               new Department { Code = "SA", Name = "Sales" },
                               new Department { Code = "AC", Name = "Accounts" },
                               new Department { Code = "AC", Name = "Accounting" },
                               new Department { Code = "HR", Name = "Human Resources" },
                               new Department { Code = "HR", Name = "Human Res." }};

        static private Department[] departments2 = { new Department { Code = "MK", Name = "Marketing" },
                               new Department { Code = "SA", Name = "Sales" },
                               new Department { Code = "IT", Name = "Information Technology" },
                               new Department { Code = "HR", Name = "Human Rsc" }};

        static private Department[] departments3 = { new Department { Code = "MK", Name = "Marketing" },
                               new Department { Code = "IT", Name = "Information Tech." },
                               new Department { Code = "SA", Name = "Sales" },
                               new Department { Code = "HR", Name = "Human Resources" }};

        #region[ List : Contains ]
        public static void Test_Contains(bool doTest)
        {
            if (!doTest) return;

            bool deptContains = departments.Contains(new Department { Code = "AC", Name = "Accts." }, new DepartmentComparer());
            Console.WriteLine("It {0} contained.", deptContains ? "is" : "isn't");
            // It is contained.
        }
        #endregion

        #region[ List : Distinct 중복제거]
        public static void Test_Distinct(bool doTest)
        {
            if (!doTest) return;

            IEnumerable<Department> deptDistinct = departments.Distinct(new DepartmentComparer());

            foreach (Department dept in deptDistinct)
            {
                Console.WriteLine("{0} {1}", dept.Code, dept.Name);
                //Console.WriteLine(dept.Code.GetHashCode());   // for testing
            }
        }
        #endregion

        #region[ List : Except 두 리스트의 차집합]
        public static void Test_Except(bool doTest)
        {
            if (!doTest) return;

            IEnumerable<Department> deptExcept = departments.Except(departments2, new DepartmentComparer());

            //Departments with Code "AC" are considered to be the same so, when displaying the Name, which name appears? The first one: "Accounts".
            foreach (Department dept in deptExcept)
            {
                Console.WriteLine("{0} {1}", dept.Code, dept.Name);
            }
            // departments not in departments2: AC, Accounts.
        }
        #endregion

        #region[ List : Intersect 두 리스트의 교집합]
        public static void Test_Intersect(bool doTest)
        {
            if (!doTest) return;

            Console.WriteLine("Intersecting departments:");

            IEnumerable<Department> deptIntersect = departments.Intersect(departments2,
                new DepartmentComparer());

            foreach (Department dept in deptIntersect)
            {
                Console.WriteLine("{0} {1}", dept.Code, dept.Name);
            }
            // MK Marketing, SA Sales, HR Human Resources
        }
        #endregion

        #region[ List : SequenceEqual 두 리스트가 순차적으로 동일하게 일치하는가? ]
        public static void Test_SequenceEqual(bool doTest)
        {
            if (!doTest) return;
            bool deptEquals = departments2.SequenceEqual(departments3, new DepartmentComparer());
            Console.WriteLine("They are{0}the same.", deptEquals ? " " : " not ");
            // They are not the same. Change them to have the same order and they
            // will be the same.
        }
        #endregion

        #region[ List : Union 두 리스트의 합집합 ]
        public static void Test_Union(bool doTest)
        {
            if (!doTest) return;
            Console.WriteLine("Union of departments and departments 2:");

            IEnumerable<Department> deptUnion = departments.Union(departments2,
                new DepartmentComparer());

            foreach (Department dept in deptUnion)
            {
                Console.WriteLine("{0} {1}", dept.Code, dept.Name);
            }
            // MK SA AC HR IT
            // (each only appears once, with AC displaying as "Accounts"
            // and HR as "Human Resources")
        }
        #endregion

        #region [ TEST ]
        public static void DoTest(bool doTest)
        {
            if (doTest)
            {
                Test_Contains(false);
                Test_Distinct(false);
                Test_Except(false);
                Test_Intersect(false);
                Test_SequenceEqual(false);
                Test_Union(true);
            }
        }
        #endregion
    }
    class CMyList
    {
        #region[ List : Find ]
        public class Part : IEquatable<Part>, IComparable<Part>
        {
            public string PartName { get; set; }
            public int PartId { get; set; }

            public override string ToString()
            {
                return "ID:" + PartId + "Name:" + PartName;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Part objAsPart = obj as Part;
                if (objAsPart == null) return false;
                else return Equals(objAsPart);
            }
            public override int GetHashCode()
            {
                return PartId;
            }
            public bool Equals(Part other)
            {
                if (other == null) return false;
                else return PartId.Equals(other.PartId);
            }

            public int CompareTo(Part other)
            {
                if (other == null)
                    return 1;
                else
                    return this.PartId.CompareTo(other.PartId);
            }
            public int SortByNameAscending(string name1, string name2)
            {
                return name1.CompareTo(name2);
            }
        }
        static void Test_Find(bool doTest)
        {
            if (!doTest) return;

            // Create a list of parts.
            List<Part> parts = new List<Part>();

            // Add parts to the list.
            parts.Add(new Part() { PartName = "crank arm", PartId = 1234 });
            parts.Add(new Part() { PartName = "chain ring", PartId = 1334 });
            parts.Add(new Part() { PartName = "regular seat", PartId = 1434 });
            parts.Add(new Part() { PartName = "banana seat", PartId = 1444 });
            parts.Add(new Part() { PartName = "cassette", PartId = 1534 });
            parts.Add(new Part() { PartName = "shift lever", PartId = 1634 }); ;

            // Write out the parts in the list. This will call the overridden ToString method
            // in the Part class.
            Console.WriteLine();
            foreach (Part aPart in parts)
            {
                Console.WriteLine(aPart);
            }


            // Check the list for part #1734. This calls the IEquatable.Equals method
            // of the Part class, which checks the PartId for equality.
            Console.WriteLine("\nContains: Part with Id=1734: {0}",
                parts.Contains(new Part { PartId = 1734, PartName = "" }));

            // Find items where name contains "seat".
            Console.WriteLine("\nFind: Part where name contains \"seat\": {0}",
                parts.Find(x => x.PartName.Contains("seat")));

            // Check if an item with Id 1444 exists.
            Console.WriteLine("\nExists: Part with Id=1444: {0}",
                parts.Exists(x => x.PartId == 1444));
            /*This code example produces the following output:
            ID: 1234   Name: crank arm
            ID: 1334   Name: chain ring
            ID: 1434   Name: regular seat
            ID: 1444   Name: banana seat
            ID: 1534   Name: cassette
            ID: 1634   Name: shift lever
            Contains: Part with Id=1734: False
            Find: Part where name contains "seat": ID: 1434   Name: regular seat
            Exists: Part with Id=1444: True 
             */
        }
        #endregion
        #region[List: TrueForAll,FindAll,FindLast]
        static void Test_ListAPI(bool doTest)
        {
            if (!doTest) return;

            List<string> dinosaurs = new List<string>();

            dinosaurs.Add("Compsognathus");
            dinosaurs.Add("Amargasaurus");
            dinosaurs.Add("Oviraptor");
            dinosaurs.Add("Velociraptor");
            dinosaurs.Add("Deinonychus");
            dinosaurs.Add("Dilophosaurus");
            dinosaurs.Add("Gallimimus");
            dinosaurs.Add("Triceratops");

            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine("\nTrueForAll(EndsWithSaurus): {0}",
                dinosaurs.TrueForAll(EndsWithSaurus));

            Console.WriteLine("\nFind(EndsWithSaurus): {0}",
                dinosaurs.Find(EndsWithSaurus));

            Console.WriteLine("\nFindLast(EndsWithSaurus): {0}",
                dinosaurs.FindLast(EndsWithSaurus));

            Console.WriteLine("\nFindAll(EndsWithSaurus):");
            List<string> sublist = dinosaurs.FindAll(EndsWithSaurus);

            foreach (string dinosaur in sublist)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine(
                "\n{0} elements removed by RemoveAll(EndsWithSaurus).",
                dinosaurs.RemoveAll(EndsWithSaurus));

            Console.WriteLine("\nList now contains:");
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine("\nExists(EndsWithSaurus): {0}",
                dinosaurs.Exists(EndsWithSaurus));
        }

        // Search predicate returns true if a string ends in "saurus".
        private static bool EndsWithSaurus(String s)
        {
            return s.ToLower().EndsWith("saurus");
        }
        #endregion

        #region[ List : Sort Basic ]
        static void Test_BasicSort(bool doTest)
        {
            if (!doTest) return;

            String[] names = { "Samuel", "Dakota", "Koani", "Saya", "Vanya",
                         "Yiska", "Yuma", "Jody", "Nikita" };
            var nameList = new List<String>();
            nameList.AddRange(names);
            Console.WriteLine("List in unsorted order: ");
            foreach (var name in nameList)
                Console.Write("   {0}", name);

            Console.WriteLine(Environment.NewLine);

            nameList.Sort();
            Console.WriteLine("List in sorted order: ");
            foreach (var name in nameList)
                Console.Write("   {0}", name);

            Console.WriteLine();
        }
        #endregion

        #region[ List : Sort with delegate or Comparator ]
        // Part class의 Interface 상속이 중요한 역할을 하니 참고바람
        static void Test_ExtendedSort(bool doTest)
        {
            if (!doTest) return;
            // Create a list of parts.
            List<Part> parts = new List<Part>();

            // Add parts to the list.
            parts.Add(new Part() { PartName = "regular seat", PartId = 1434 });
            parts.Add(new Part() { PartName = "crank arm", PartId = 1234 });
            parts.Add(new Part() { PartName = "shift lever", PartId = 1634 }); ;
            // Name intentionally left null.
            parts.Add(new Part() { PartId = 1334 });
            parts.Add(new Part() { PartName = "banana seat", PartId = 1444 });
            parts.Add(new Part() { PartName = "cassette", PartId = 1534 });


            // Write out the parts in the list. This will call the overridden 
            // ToString method in the Part class.
            Console.WriteLine("\nBefore sort:");
            foreach (Part aPart in parts)
            {
                Console.WriteLine(aPart);
            }


            // Call Sort on the list. This will use the 
            // default comparer, which is the Compare method 
            // implemented on Part.
            parts.Sort();


            Console.WriteLine("\nAfter sort by part number:");
            foreach (Part aPart in parts)
            {
                Console.WriteLine(aPart);
            }

            // This shows calling the Sort(Comparison(T) overload using 
            // an anonymous method for the Comparison delegate. 
            // This method treats null as the lesser of two values.
            parts.Sort(delegate (Part x, Part y)
            {
                if (x.PartName == null && y.PartName == null) return 0;
                else if (x.PartName == null) return -1;
                else if (y.PartName == null) return 1;
                else return x.PartName.CompareTo(y.PartName);
            });

            Console.WriteLine("\nAfter sort by name:");
            foreach (Part aPart in parts)
            {
                Console.WriteLine(aPart);
            }

        }
        #endregion

        #region[ List : ToArray,AddRange, RemoveRange, InsertRange ]
        static void Test_ToArray(bool doTest)
        {
            if (!doTest) return;
            string[] input = { "Brachiosaurus",
                           "Amargasaurus",
                           "Mamenchisaurus" };

            List<string> dinosaurs = new List<string>(input);

            Console.WriteLine("\nCapacity: {0}", dinosaurs.Capacity);

            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine("\nAddRange(dinosaurs)");
            dinosaurs.AddRange(dinosaurs);

            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine("\nRemoveRange(2, 2)");
            dinosaurs.RemoveRange(2, 2);

            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            input = new string[] { "Tyrannosaurus",
                               "Deinonychus",
                               "Velociraptor"};

            Console.WriteLine("\nInsertRange(3, input)");
            dinosaurs.InsertRange(3, input);

            Console.WriteLine();
            foreach (string dinosaur in dinosaurs)
            {
                Console.WriteLine(dinosaur);
            }

            Console.WriteLine("\noutput = dinosaurs.GetRange(2, 3).ToArray()");
            string[] output = dinosaurs.GetRange(2, 3).ToArray();

            Console.WriteLine();
            foreach (string dinosaur in output)
            {
                Console.WriteLine(dinosaur);
            }
        }

        #endregion

        #region[ List : 중복제거 ]
        static void Test_RemoveDuplicateElement(bool doTest)
        {
            if (!doTest) return;

            int[] a = { 1, 2, 3, 4, 5 };
            int[] b = { 4, 5, 6, 7, 8, 9, 10 };
            List<int> aa = new List<int>(a);
            List<int> bb = new List<int>(b);

            List<int> result = new List<int>();
            result.AddRange(aa);
            result.AddRange(bb);

            Console.WriteLine("Original list");
            foreach (int item in result)
            {
                Console.Write(item);
            }
            Console.WriteLine();

            result = result.Distinct().ToList();

            Console.WriteLine("Removed Duplicated element list");
            foreach (int item in result)
            {
                Console.Write(item);
            }
            Console.WriteLine();
        }
        #endregion

        #region [ TEST ]
        public static void DoTest(bool doTest)
        {
            if (doTest)
            {
                Test_Find(false);
                Test_ListAPI(false);
                Test_BasicSort(false);
                Test_ExtendedSort(false);
                Test_ToArray(false);
                Test_RemoveDuplicateElement(true);
            }
        }
        #endregion
    }
    class CMyDictionary
    {
        #region [ Dictionary : 기본 사용법 ]
        public static void addDictionary(bool doTest)
        {
            if (!doTest) return;

            Dictionary<string, int> dictionary = new Dictionary<string, int>
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 }
            };
            Console.WriteLine("first one value:{0}", dictionary["one"]);
        }
        #endregion
        #region [ ContainsKey : 주어진 문자열이 Dictionary에 존재하면 true 없으면 false 리턴 ]
        public static void Test_ContainKey(bool doTest)
        {
            if (!doTest) return;

            Dictionary<string, int> dict = new Dictionary<string, int>()
            {
                {"apple",1 },
                {"window",2 }
            };

            if (dict.ContainsKey("apple"))
            {
                int val = dict["apple"];
                Console.WriteLine(val);
            }
            if (!dict.ContainsKey("arcorn"))
            {
                Console.WriteLine(false);
            }
        }
        #endregion

        #region[ TryGetValue : 효율적인 검색 방법 ]
        public static void Test_TryGetValue(bool doTest)
        {
            if (!doTest) return;
            Dictionary<string, string> dc = new Dictionary<string, string>()
            {
                {"cat","feline" },{"dog","canline"}
            };
            string result;
            if (dc.TryGetValue("cat", out result))
            {
                Console.WriteLine(result);
            }
            if (dc.TryGetValue("dog", out string result2))
            {
                Console.WriteLine(result2);
            }
        }
        #endregion
        #region [ Dictionary loop ]
        public static void Test_Loop(bool doTest)
        {
            if (!doTest) return;
            Dictionary<string, int> dc = new Dictionary<string, int>()
            {
                {"one",1 },
                {"two",2 },
                {"three",3 },
                {"four",4 },
                {"five",5 }
            };

            foreach (KeyValuePair<string, int> pair in dc)
            {
                Console.WriteLine("{0},{1}", pair.Key, pair.Value);
            }

            foreach (var pair in dc)
            {
                Console.WriteLine("{0},{1}", pair.Key, pair.Value);
            }

            List<string> list = new List<string>(dc.Keys);
            foreach (string k in list)
            {
                Console.WriteLine("{0},{1}", k, dc[k]);
            }

        }
        #endregion
        #region[ convert array to Dictionary ]
        public static void Test_Array2Dict(bool doTest)
        {
            if (!doTest) return;
            string[] arr = new string[]
            {
                "One",
                "Two",
                "Three"
            };

            var dict = arr.ToDictionary(item => item, item => true);
            foreach (var pair in dict)
            {
                Console.WriteLine("{0},{1}", pair.Key, pair.Value);
            }

        }
        #endregion
        #region [ TEST ]
        public static void DoTest(bool doTest)
        {
            if (doTest)
            {
                addDictionary(false);
                Test_ContainKey(false);
                Test_TryGetValue(false);
                Test_Loop(false);
                Test_Array2Dict(true);
            }
        }
        #endregion
    }
}
