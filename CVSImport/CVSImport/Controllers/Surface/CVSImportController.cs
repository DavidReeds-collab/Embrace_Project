using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
//Added system.IO for input output generating. 
using System.IO;
//Standard microsoft namespace for validating email adresses.
using System.Net.Mail;
//used for logging. 
using System.Text;

//Minor typo here; should be CSVImport
namespace CVSImport.Controllers.Surface
{
    //Minor typo here; should be CSVImport. If fixed, change URL to reflect the change. 
    public class CVSImportController : SurfaceController
    {
        //Called from http://localhost:1471/umbraco/Surface/CVSImport/ImportCSV
        public ActionResult ImportCSV()
        {
            List<int> userNameIterators = new List<int>();

            List<int> nameIterators = new List<int>();

            List<int> emailIterators = new List<int>();

            StringBuilder log = new StringBuilder();

            //Using a stream to get the file from a locked location, then splitting it up into c# objects that can be added via the extended and exposed service. 
            //Todo: does this need to be a hard location?
            using (var reader = new StreamReader(@"D:\Documents\Embrace_Project\MOCK_DATA.csv"))
            {
                int currentLine = 0;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    string[] values = line.Split(',');

                    //checks the first line of the CSV for the columns present and gets the column number of relevant columns. 
                    if (currentLine == 0)
                    {
                        //These strings will be used to find the column names that will form the username. 
                        List<string> userNameComponents = new List<string>() { "first_name", "last_name" };

                        //These strings will be used to find the column names that will form the name. 
                        List<string> nameComponents = new List<string>() { "first_name", "last_name" };

                        //These strings will be used to find the column names that will form the Email. 
                        List<string> emailComponents = new List<string>() { "email" };

                        for (int i = 0; i < values.Length; i++)
                        {
                            if (userNameComponents.Contains(values[i]))
                            {
                                userNameIterators.Add(i);
                            }
                            if (nameComponents.Contains(values[i]))
                            {
                                nameIterators.Add(i);
                            }
                            if (emailComponents.Contains(values[i]))
                            {
                                emailIterators.Add(i);
                            }
                        }

                        currentLine++;

                        continue;
                    }

                    Member newMember = new Member();

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (userNameIterators.Contains(i))
                        {
                            newMember.Username += values[i];
                        }
                        if(nameIterators.Contains(i))
                        {
                            newMember.Name += values[i] + " ";
                        }
                        if (emailIterators.Contains(i))
                        {
                            newMember.Email += values[i];
                        }
                    }

                    if (IsValidNewMember(newMember))
                    {
                        Services.MemberService.CreateMemberWithIdentity(newMember.Username, newMember.Email, newMember.Name, "Member");

                        log.AppendFormat($"Succesfully added new member {newMember.Username} ({newMember.Email}).<br />");
                    }
                    else
                    {
                        log.AppendFormat($"failed to add new member {newMember.Username} ({newMember.Email}).<br />");
                    }

                    currentLine++;
                }
            }

            log.AppendLine("Done.");

            return Content(log.ToString());
        }


        //This should be a service. If such a service already exists, replace this one with the existing one. 
        public bool IsValidNewMember(Member member)
        {
            if (IsValidEmail(member.Email) && IsNewMember(member))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //This should be a service. If such a service already exists, replace this one with the existing one. 
        //Done as reccomended by Microsoft.
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        //This should be a service. If such a service already exists, replace this one with the existing one. 
        public bool IsNewMember(Member member)
        {
            var newMember = Services.MemberService.GetByEmail(member.Email);

            if (newMember == null)
            {
                newMember = Services.MemberService.GetByUsername(member.Username);

                if (newMember == null)
                {
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }
    }

    //Can this be replaced with the IMember Interface, or is a more isolated model better suited here?
    public class Member
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}