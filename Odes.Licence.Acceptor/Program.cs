using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Rhino.Licensing;
using Odes.Licence.Model;
using Dapper;

namespace Odes.Licence.Accept
{
    class Program
    {
        private static bool _proceed;
        private static string _savedFileLocation;
        private static FileStream _file;
        private static readonly Model.Licence Licx  = new Model.Licence();
        private static bool _hasEnded;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the QCAT Object Coding Licence acceptance program");
            Console.WriteLine("---------------------------------------------------------");
            Console.WriteLine();

            _proceed = WelcomeAndStart();

            if (StopOrContinue(_proceed)) return;

            _proceed = EnterDatabaseServerName();

            if (StopOrContinue(_proceed)) return;

            _proceed = EnterDatabaseName();

            if (StopOrContinue(_proceed)) return;

            _proceed = EnterDatabaseUserId();

            if (StopOrContinue(_proceed)) return;

            _proceed = EnterDatabasePassword();

            if (StopOrContinue(_proceed)) return;

            _proceed = EnterTheQcatLicenseFileWeSentYou();

            if (StopOrContinue(_proceed)) return;

            _proceed = LoadPublicKeyFragment();

            if (StopOrContinue(_proceed)) return;

            LastChanceToChangeBeforeCreatingLicenseEntry();
        }

        private static void LastChanceToChangeBeforeCreatingLicenseEntry()
        {
            PrintInputs();
            Console.WriteLine("Press F to continue and create your licence entry in your QCAT database or");
            Console.WriteLine("print one of the following options to fix your inputs");
            Console.WriteLine("S for database Server Name");
            Console.WriteLine("D for database Name");
            Console.WriteLine("U for database User ID");
            Console.WriteLine("P for database Password");
            Console.WriteLine("L for path of Licence file we sent you");

            var nextMove = Console.ReadLine().ToUpper();

            MakeMove(nextMove);
        }

        private static void MakeMove(string nextMove)
        {
            var continueWith = false;

            switch (nextMove)
            {
                case "S":
                    continueWith = EnterDatabaseServerName();
                    break;
                case "D":
                    continueWith = EnterDatabaseName();
                    break;
                case "U":
                    continueWith = EnterDatabaseUserId();
                    break;
                case "P":
                    continueWith = EnterDatabasePassword();
                    break;
                case "L":
                    continueWith = EnterTheQcatLicenseFileWeSentYou();
                    break;
                case "F":
                    PrepareLicenceDetailsForValidation();
                    break;
                case "Q":
                    Console.WriteLine();
                    Console.WriteLine("Quitting licence application!");
                    Console.WriteLine("Press any key to end!");
                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine();
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option selected!".ToUpper());
                    Console.ResetColor();
                    LastChanceToChangeBeforeCreatingLicenseEntry();
                    break;
            }

            if (continueWith)
            {
                LastChanceToChangeBeforeCreatingLicenseEntry();
                return;
            }
        }

        private static void PrepareLicenceDetailsForValidation()
        {
            try
            {
                var licenseValidator = new LicenseValidator(Licx.PublicKeyFragment, Licx.PublicallySignedFileLocation);

                licenseValidator.TryLoadingLicenseValuesFromValidatedXml();

                //quits on false
                if (ExitIfProcessIsInvalidForOutOfDateTrial(licenseValidator)==false)
                    LicenceCanBeValidatedAndSavedToDatabase(licenseValidator);
            }
            catch (Exception ex)
            {
                //program will end
                Licx.ProcessingError = ex.StackTrace;
                PrintProcessingError();
                SaveProcessingErrorToDisk();
                StopOrContinue(false);
            }
        }

        private static bool ExitIfProcessIsInvalidForOutOfDateTrial(LicenseValidator licenseValidator)
        {
            if (licenseValidator.LicenseType == LicenseType.Trial && licenseValidator.ExpirationDate > DateTime.Now)
            {
                var msg = string.Format("Licence is invalid as the date {0} is invalid!",
                                        licenseValidator.ExpirationDate);
                Console.WriteLine(msg);
                Console.WriteLine();
                Licx.ProcessingError = msg;
                PrintProcessingError();
                SaveProcessingErrorToDisk();
                StopOrContinue(false);
                return true;
            }
            return false;
        }

        private static void LicenceCanBeValidatedAndSavedToDatabase(LicenseValidator licenseValidator)
        {
            try
            {
                licenseValidator.AssertValidLicense();
                SaveLicenceToDatabase(licenseValidator);
            }
            catch (Exception ex)
            {
                Licx.ProcessingError = ex.StackTrace;
                PrintProcessingError();
                SaveProcessingErrorToDisk();
                StopOrContinue(false);
            }
        }

        private static void SaveLicenceToDatabase(LicenseValidator licenseValidator)
        {
            try
            {
                //save to db and print
                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
                sqlConnectionStringBuilder["Data Source"] = Licx.DatabaseServerName;
                sqlConnectionStringBuilder["Initial Catalog"] = Licx.DatabaseName;
                sqlConnectionStringBuilder["User Id"] = Licx.DatabaseServerUserId;
                sqlConnectionStringBuilder["Password"] = Licx.DatabaseServerPassword;

                using (var con = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
                {
                    const string insert = "insert Licences " +
                                          "([LicenceName] "+
                                          ",[LicenceIssued]"+
                                          ",[Status]"+
                                          ",[LicenceType]"+
                                          ",[ClicksIssued]"+
                                          ",[ClicksUsed]) " +
                                          "values "+
                                          "(" +
                                          "@LicenceName,"+
                                          "@LicenceIssued,"+
                                          "@Status,"+
                                          "@LicenceType,"+
                                          "@ClicksIssued,"+
                                          "@ClicksUsed)";
                    con.Open();
                    con.Execute(insert, new
                        {
                            LicenceName = string.Format("Licence Purchased on {0}", DateTimeOffset.UtcNow),
                            LicenceIssued = Licx.DatabaseLicenceFragment,
                            Status = "Available",
                            LicenceType = Licx.LicenceIssued.LicenceType,
                            ClicksIssued = Licx.LicenceIssued.ClicksReqeusted,
                            ClicksUsed = 0
                        });
                }

                PrintEndDetails(licenseValidator);
                StopOrContinue(false);
            }
            catch (Exception ex)
            {
                Licx.ProcessingError = ex.StackTrace;
                PrintProcessingError();
                SaveProcessingErrorToDisk();
                StopOrContinue(false);
            }
        }

        private static void PrintEndDetails(LicenseValidator licenseValidator)
        {
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("License Validation Details Saved To QCAT Master Database");
            Console.WriteLine("======================================================.");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(licenseValidator.ToString());
            Console.WriteLine(licenseValidator.LicenseType);
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("======================================================.");
            Console.WriteLine();
        }

        private static void PrintInputs()
        {
            Console.WriteLine();
            Console.WriteLine("Final chance to check your inputs are correct before attempting to commit your license details");
            Console.WriteLine("======================================================.");
            Console.WriteLine();

            ColorLabelInConsole();
            Console.Write("SERVER NAME: \t");
            ColorValueInConsole();
            Console.WriteLine(Licx.DatabaseServerName);

            ColorLabelInConsole();
            Console.Write("DB NAME: \t");
            ColorValueInConsole();
            Console.WriteLine(Licx.DatabaseName);

            ColorLabelInConsole();
            Console.Write("USER ID: \t");
            ColorValueInConsole();
            Console.WriteLine(Licx.DatabaseServerUserId);

            ColorLabelInConsole();
            Console.Write("DB PASSWORD: \t");
            ColorValueInConsole();
            Console.WriteLine(Licx.DatabaseServerPassword);

            ColorLabelInConsole();
            Console.Write("FILE PATH: \t");
            ColorValueInConsole();
            Console.WriteLine(Licx.PublicallySignedFileLocation);

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("======================================================.");
        }

        private static void ColorValueInConsole()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        private static void ColorLabelInConsole()
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static bool LoadPublicKeyFragment()
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly()
                               .GetManifestResourceStream("Odes.Licence.Accept.pubkey.xml"))
                {
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            Licx.PublicKeyFragment = reader.ReadToEnd();
                        }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Licx.ProcessingError = ex.StackTrace;
                SaveProcessingErrorToDisk();
                PrintProcessingError();
                return false;
            }    
        }

        private static void LoadPubliclySignedFile()
        {
            var file = File.OpenRead(Licx.PublicallySignedFileLocation);
            var fileBytes = new byte[file.Length];
            file.Read(fileBytes, 0, (int)file.Length);
            var xml = Encoding.UTF8.GetString(fileBytes);

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var fragmentToSave = new DatabaseLicenseFragment
            {
                ClicksReqeusted = Convert.ToInt32(doc.DocumentElement.Attributes["ClicksReqeusted"].Value),
                CompanyName = doc.DocumentElement.Attributes["CompanyName"].Value,
                Email = doc.DocumentElement.Attributes["Email"].Value,
                LicenceType = doc.DocumentElement.Attributes["LicenceType"].Value,
                ServiceQueue = doc.DocumentElement.Attributes["ServiceQueue"].Value,
                SystemDateTimeStamp = doc.DocumentElement.Attributes["SystemDateTimeStamp"].Value,
                SystemId = doc.DocumentElement.Attributes["SystemId"].Value,
                SystemMachineName = doc.DocumentElement.Attributes["SystemMachineName"].Value,
                SystemNetworkCredential = doc.DocumentElement.Attributes["SystemNetworkCredential"].Value,
                UserName = doc.DocumentElement.Attributes["UserName"].Value,
                expiration = doc.DocumentElement.Attributes["expiration"].Value,
                id = doc.DocumentElement.Attributes["id"].Value,
                type = doc.DocumentElement.Attributes["type"].Value
            };

            Licx.LicenceIssued = fragmentToSave;
            
            Licx.DatabaseLicenceFragment = xml;
        }

        private static void SaveProcessingErrorToDisk()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var contents = JsonConvert.SerializeObject(Licx);
            var datePart = DateTime.Now.ToString("ddMMyyyyhhmmss");
            _savedFileLocation = path + @"\odes.licx.error." + datePart + @".txt";
            _file = new FileStream(_savedFileLocation, FileMode.Create);
            var fileBytes = Encoding.UTF8.GetBytes(contents);
            _file.BeginWrite(fileBytes, 0, fileBytes.Length, UserCallback, contents);
            while (!_hasEnded) { }
        }

        private static void UserCallback(IAsyncResult ar)
        {
            _file.EndWrite(ar);
            _file.Close();
            _file.Dispose();
            Console.WriteLine();
            Console.WriteLine("Finished saving licence request file to: {0}", _savedFileLocation);
            Console.WriteLine("Press any key to end");
            Console.WriteLine();
            _hasEnded = true;
        }

        private static void PrintProcessingError()
        {
            Console.WriteLine();
            Console.WriteLine("*****************************************");
            Console.WriteLine();
            Console.WriteLine("Unexpected processing error!");
            Console.WriteLine("Error data: {0}", Licx.ProcessingError);
            Console.WriteLine();
            Console.WriteLine("*****************************************");
            Console.WriteLine();
        }

        private static bool EnterTheQcatLicenseFileWeSentYou()
        {
            var answered = false;
            string publicallySignedFileLocation;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please supply the fully qaulified path to the QCAT licence file we sent you!");
                Console.WriteLine("NOTE: It ends with a .xml extension!");

                publicallySignedFileLocation = Console.ReadLine();

                if (string.IsNullOrEmpty(publicallySignedFileLocation) == false && File.Exists(publicallySignedFileLocation))
                {
                    answered = true;
                    Licx.PublicallySignedFileLocation = publicallySignedFileLocation;
                    try
                    {
                        LoadPubliclySignedFile();
                    }
                    catch (Exception ex)
                    {
                        Licx.ProcessingError = ex.StackTrace;
                        PrintProcessingError();
                        SaveProcessingErrorToDisk();
                        return false;
                    }
                }

            } while (!answered);

            return publicallySignedFileLocation != "Q";
        }

        private static bool EnterDatabasePassword()
        {
            var answered = false;
            string databasePassword;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please supply us your database password!");

                databasePassword = Console.ReadLine();

                if (string.IsNullOrEmpty(databasePassword) == false)
                {
                    answered = true;
                    Licx.DatabaseServerPassword = databasePassword;
                }

            } while (!answered);

            return databasePassword != "Q";
        }

        private static bool EnterDatabaseUserId()
        {
            var answered = false;
            string databaseServerUserId;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please supply us your database User ID!");

                databaseServerUserId = Console.ReadLine();

                if (string.IsNullOrEmpty(databaseServerUserId) == false)
                {
                    answered = true;
                    Licx.DatabaseServerUserId = databaseServerUserId;
                }

            } while (!answered);

            return databaseServerUserId != "Q";
        }

        private static bool EnterDatabaseServerName()
        {
            var answered = false;
            string databaseServerName;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please supply us your database Server Name!");

                databaseServerName = Console.ReadLine();

                if (string.IsNullOrEmpty(databaseServerName) == false)
                {
                    answered = true;
                    Licx.DatabaseServerName = databaseServerName;
                }

            } while (!answered);

            return databaseServerName != "Q";
        }

        private static bool EnterDatabaseName()
        {
            var answered = false;
            string databaseName;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Please supply us your database Name!");

                databaseName = Console.ReadLine();

                if (string.IsNullOrEmpty(databaseName) == false)
                {
                    answered = true;
                    Licx.DatabaseName = databaseName;
                }

            } while (!answered);

            return databaseName != "Q";
        }

        private static bool StopOrContinue(bool proceed)
        {
            if (!proceed)
            {
                Exiting();
                return true;
            }
            return false;
        }

        private static void Exiting()
        {
            Console.WriteLine("Press any key to exit. Goodbye!");
            Console.ReadLine();
        }

        private static bool WelcomeAndStart()
        {
            var answered = false;
            string answer;

            do
            {
                Console.WriteLine("Would you like to accept a new licence? Y for Yes. Press Q at any time to quit");
                answer = Console.ReadLine().ToUpper();

                if (answer == "Y" || answer == "Q")
                    answered = true;


            } while (!answered);

            return answer == "Y";
        }
    }
}