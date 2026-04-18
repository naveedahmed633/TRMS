using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenerateValidationKey
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string strKey = "";

            ////getImagesListByNamePart();

            ////return;

            // _ _ _ _ _ - _ _ _ 1 _ - _ 9 _ _ _ - 0 _ _ _ _ - _ _ 2 _ _ - _ _ _ _ _

            Console.WriteLine("Processing to Generate Key. Please wait...");
            strKey = GenerateAccessKey();

            Console.WriteLine(strKey);
            System.Threading.Thread.Sleep(100);
            Clipboard.SetText(strKey);

            Console.WriteLine("\nThe generated key is copied to clipboard. Just press Ctrl+V to paste.\n");

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        public static string GenerateAccessKey()
        {
            string key = ""; int number = 0;
            string currentYear = DateTime.Now.Year.ToString();
            for (int i = 1; i <= 35; i++)
            {
                if (i == 6 || i == 12 || i == 18 || i == 24 || i == 30)
                {
                    key += "-";
                }
                else
                {
                    AttributeCol:
                    number = RandomNumber(48, 90);
                    if (number >= 58 && number <= 64)
                    {
                        goto AttributeCol;
                    }

                    if (i == 10)
                    {
                        key += ReplaceNumber(currentYear[2].ToString());//1
                    }
                    else if (i == 14)
                    {
                        key += ReplaceNumber(currentYear[3].ToString());//9
                    }
                    else if (i == 19)
                    {
                        key += ReplaceNumber(currentYear[1].ToString());//0
                    }
                    else if (i == 27)
                    {
                        key += ReplaceNumber(currentYear[0].ToString());//2
                    }
                    else
                    {
                        key += (char)number;
                    }
                }
            }

            return key.ToString();
        }

        //Generate a random number between two numbers
        public static int RandomNumber(int min, int max)
        {
            int number = 0;

            System.Threading.Thread.Sleep(100);

            Random random = new Random();
            number = random.Next(min, max);

            return number;
        }

        //replace number
        public static char ReplaceNumber(string n)
        {
            char c = 'M';

            if (n == "0")
                c = 'M';
            else if (n == "1")
                c = 'K';
            else if (n == "2")
                c = 'P';
            else if (n == "3")
                c = 'S';
            else if (n == "4")
                c = 'F';
            else if (n == "5")
                c = 'B';
            else if (n == "6")
                c = 'Z';
            else if (n == "7")
                c = 'V';
            else if (n == "8")
                c = 'W';
            else if (n == "9")
                c = 'U';

            return c;
        }

        private static void getImagesListByNamePart()
        {
            string partialInputName = "000001"; //textbox input value or whatever you want to input
            string[] directories = Directory.GetDirectories(@"E:\UNIS\JPG");
            List<FileInfo> fileinDir;
            foreach (string dir in directories)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if (dirInfo.Exists)
                {
                    //taking the first (FirstOrDefault()), considering that all files have a unique name with respect to the input value that you are giving. so it should fetch only one file every time you query
                    fileinDir = dirInfo.GetFiles("20200107*" + partialInputName + ".jpg").ToList();

                    foreach (var f in fileinDir)
                    {
                        Console.WriteLine(f.FullName);
                    }
                   
                }
            }
        }


        private static void DB_Stored_Procedure_Random()
        {
            /*
            
		USE [TRMS_DUHS]
GO
// Object:  StoredProcedure [dbo].[SP_CheckAccessValidationRandom]    Script Date: 10/16/2019 10:15:20 AM //
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- [dbo].[SP_CheckAccessValidationRandom] 'localhost'
-- [dbo].[SP_CheckAccessValidationRandom] ''

ALTER Procedure [dbo].[SP_CheckAccessValidationRandom]
@URL NVarChar(100)=''
--WITH ENCRYPTION
AS
BEGIN

	--SET @URL=Replace(@URL,'localhost','')

	DECLARE @URLSubWord as NVarChar(100) = ''
	SET @URLSubWord='host'

	DECLARE @LicenseKey as NVarChar(35) = ''
	DECLARE @IsValidKey as INT = 0

	--SELECt GetDate()

	IF @LicenseKey=''
		SELECT @LicenseKey=AccessValue FROM AccessCodeValues WHERE AccessCode='ACCESS_VALIDATION'

	IF LEN(@LicenseKey) = 35
	BEGIN
		----Print @LicenseKey

		-- SELECT ROUND(RAND()*999999,0);
		-- SELECT CHARINDEX('2019',GetDate());
		-- SELECT (YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))

		DECLARE @YearPIN CHAR(4);
		SELECT @YearPIN=Cast(Year(GetDate()) as CHAR(4));
		----Print '>>'+@YearPIN;

		DECLARE @LicKeyPIN CHAR(4);
		DECLARE @LicKeyP1N CHAR(1); SELECT @LicKeyP1N=SUBSTRING(@LicenseKey,27,1);--2
		DECLARE @LicKeyP2N CHAR(1); SELECT @LicKeyP2N=SUBSTRING(@LicenseKey,19,1);--0
		DECLARE @LicKeyP3N CHAR(1); SELECT @LicKeyP3N=SUBSTRING(@LicenseKey,10,1);--1
		DECLARE @LicKeyP4N CHAR(1); SELECT @LicKeyP4N=SUBSTRING(@LicenseKey,14,1);--9

		--XKY4I-2NRKR-8U6P2-M48RF-CRMQP
		SET @LicKeyPIN=@LicKeyP1N+@LicKeyP2N+@LicKeyP3N+@LicKeyP4N;
		SET @LicKeyPIN=REPLACE(@LicKeyPIN,'M','0');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'K','1');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'P','2');
		SET @LicKeyPIN=REPLACE(@LicKeyPIN,'S','3');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'F','4');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'B','5');
		SET @LicKeyPIN=REPLACE(@LicKeyPIN,'Z','6');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'V','7');SET @LicKeyPIN=REPLACE(@LicKeyPIN,'W','8');
		SET @LicKeyPIN=REPLACE(@LicKeyPIN,'U','9');
		----Print '>>>'+@LicKeyPIN;

		----@LicenseKey IN('XKY4I-2NRKR-8U6P2-M48RF-CRPMQ','TK8TP-9JN6P-7X3WW-RFFTV-B7QPF','QXV7B-K68W2-QGPR6-9FWH9-KGMZ7')
		IF CHARINDEX(@URLSubWord,@URL) > 0 AND @LicKeyPIN=@YearPIN
			SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
		ELSE
			SET @IsValidKey=0

		SELECT @IsValidKey as IsValidKey
	END
	ELSE
	BEGIN
		SELECT 0 as IsValidKey
	END

End

            */
        }


        private static void DB_Stored_Procedure_Fixed_By_Sheet()
        {
            /*
            USE [TRMS_DUHS]
            GO
            -- Object:  StoredProcedure [dbo].[SP_CheckAccessValidation]    Script Date: 10/04/2020 03:28:31 PM 
            SET ANSI_NULLS ON
            GO
            SET QUOTED_IDENTIFIER ON
            GO

            --	[dbo].[SP_CheckAccessValidation] 'localhost'
            --	[dbo].[SP_CheckAccessValidation] ''

            ALTER Procedure [dbo].[SP_CheckAccessValidation]
	            @URL NVarChar(100)=''
            --WITH ENCRYPTION
            AS
            BEGIN
	
	            --SET @URL=Replace(@URL,'localhost','')

	            DECLARE @URLSubWord as NVarChar(100) = ''
	            SET @URLSubWord='host'
		
	            DECLARE @LicenseKey as NVarChar(30) = ''
	            DECLARE @IsValidKey as INT = 0

	            --SELECt GetDate()

	            IF @LicenseKey=''
		            SELECT @LicenseKey=AccessValue FROM AccessCodeValues WHERE AccessCode='ACCESS_VALIDATION'

	            Print @LicenseKey

	            --	SELECT ROUND(RAND()*999999,0);
	            --	SELECT CHARINDEX('2019',GetDate());
	            --	SELECT (YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))

	            --E3R2
	            IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2019',GetDate()) > 0 AND @LicenseKey IN('XKY4I-2NRWR-8F6P2-448RF-CRMQH','TK8TP-9JN6P-7X3WW-RFFTV-B7QPF','QXV7B-K68W2-QGPR6-9FWH9-KGMZ7')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2020',GetDate()) > 0 AND @LicenseKey IN('RR3BN-3YA3P-9D7FC-7J4YF-QGJXW','FB4WR-72NVD-4RW73-XQFWH-CYQG3','32JNW-4KQ84-P47T8-D8RGY-CWCK7')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2021',GetDate()) > 0 AND @LicenseKey IN('XCVCF-2YXM9-7Z3PB-MHCB7-2RYQQ','JWNMF-RHW7P-DMY6X-RF3DR-X5BQT','9G4YW-VH26C-733KU-K6F68-J8CK4')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2022',GetDate()) > 0 AND @LicenseKey IN('327NW-9KQ84-P47T8-D8EGY-CWCK7','TK8TP-9J96P-7X7WW-RSFTV-B7PQFA','XWCHQ-CDMYC-9WN2C-BQWTV-YY2KV')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2023',GetDate()) > 0 AND @LicenseKey IN('NG4HW-VH26C-703KW-K6F98-J8CK4','HMGNV-WCYXV-X7B9W-YCX63-B98R2','2XNFG-KFHR8-QV3CP-3W6HT-682CH')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2024',GetDate()) > 0 AND @LicenseKey IN('HM6NR-QEX7C-DFW2Y-8B82K-WTYJV','22TKD-F8OX6-YG69F-9M66D-PMJBM','342DG-6YJR8-X92GV-V7DCV-P4K21')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2025',GetDate()) > 0 AND @LicenseKey IN('FJGCP-4DFJD-GJY49-VJBQ7-HYRD2','BWG7X-J98B3-W34RT-34B3R-JVYW9','XDM3T-W3T3V-MGJHK-8BFVD-GVPKY')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2026',GetDate()) > 0 AND @LicenseKey IN('FBJVC-3CMTX-D8DVP-RTQCT-924K4','2VTNH-3T3J4-BWP98-TX9JR-FCWXV','84NRV-6CJR6-DBDXH-FYTBF-4C49V')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2027',GetDate()) > 0 AND @LicenseKey IN('MHF9N-VY6XB-WVXMC-BTDCT-MKKG7','TGXN4-BPPYC-TJYMH-3WXFK-4JNQH','NTTX3-RV7VB-T7X7F-WQYYY-9YE2F')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2028',GetDate()) > 0 AND @LicenseKey IN('N9C46-MKLKR-2TTT8-FJCJP-4RDG7','982NM-XKXT9-7YFWH-H2Q3Q-C34DH','4NJMK-QJH7K-F38H2-FQJ24-2J8XV')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2029',GetDate()) > 0 AND @LicenseKey IN('AKY4I-2NRWR-8F6P2-448RF-CRMQH','TK8TP-BJN6P-7X3WW-RFFTV-B7QPF','QXV7B-K68W2-CGPR6-9FWH9-KGMZ7')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('2030',GetDate()) > 0 AND @LicenseKey IN('RR3BN-3YA3P-9D7FC-DJ4YF-QGJXW','FB4WR-72NVD-4RW73-XQFWH-EYQG3','F2JNW-4KQ84-P47T8-D8RGY-CWCK7')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2031',GetDate()) > 0 AND @LicenseKey IN('4CPRK-NM3K3-X6XXQ-RKJ86-WXCHW','QTFDN-GRT3P-VKLWS-X7T3R-8B639','VK7JG-NPHTM-C97JM-9MPGV-3V06T')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2032',GetDate()) > 0 AND @LicenseKey IN('DCPHK-NFMTC-H88MJ-PFHQY-QJ4BJ','YTYG3-N6DKC-DKB07-7M9GH-8HVX7','2F37B-TNFGY-69BQF-B8YKP-D69TJ')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2033',GetDate()) > 0 AND @LicenseKey IN('XLG7C-N36Q4-C4HTG-X4T3X-2YV57','WNMTR-4C6BP-JK8YV-HQ7T2-76DF9','7YPNQ-8C467-V2W6J-TR4AX-WT2RQ')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2034',GetDate()) > 0 AND @LicenseKey IN('DPH2V-TXNVB-4X9Q3-TJR4H-K9JW4','YNMGQ-8RYV3-4PGQ3-C8XTP-7CFQY','43RPN-FTY23-9VOTB-MP9BX-TU4FV')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2035',GetDate()) > 0 AND @LicenseKey IN('84NGF-MHBT6-JXBC8-QWJK7-DKR8H','NW6C2-QMPVS-D7KSW-3GKT6-VCFB2','UPTN6-RNW2C-6V7J2-C2D7X-MHBPE')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2036',GetDate()) > 0 AND @LicenseKey IN('YVWGF-BXNMC-HTHYQ-CPQE9-66QFC','GJTYN-HDMQY-FRE76-HVGC7-QPF8P','WXV89-NTFWV-6MDZ3-9PT4G-4M68B')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2037',GetDate()) > 0 AND @LicenseKey IN('XGVPL-NMH47-7TTHJ-W3FW7-8HV2C','NPBR9-FWDCX-D2C8J-H872K-2YT43','MNXKQ-WY2CT-JWBO2-T68TQ-Y1H2V')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2038',GetDate()) > 0 AND @LicenseKey IN('NW6C2-QMPVW-D7KCA-3GKT6-VCFB2','MNXKQ-WY2CT-JWBS2-T68TQ-YBH2V','DCPHK-NFMTC-H8QMJ-PFHLY-QJ4BJ')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2039',GetDate()) > 0 AND @LicenseKey IN('WYPNQ-8C467-V2W6J-TX4WA-UT2RQ','QFTDN-GRT3P-VKWWX-X7T3R-8B639','84NGF-MHBT6-FXBX8-QWJK7-DR08H')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
	            ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND CHARINDEX('2040',GetDate()) > 0 AND @LicenseKey IN('2F77B-TNFGY-69ZQF-B8YKP-D69TJ','VK7JG-NPHTM-C97JM-9MPGT-3V96E','WNMTR-4C20C-JK8YV-HQ7T2-76DF9')
		            SET @IsValidKey=(YEAR(GetDate())-1111-(MONTH(GetDate())+DAY(GetDate())))*ROUND(RAND()*999999,0)
            --	ELSE IF CHARINDEX(@URLSubWord,@URL) > 0 AND  CHARINDEX('20',GetDate()) > 0 
            --		SET @IsValidKey=YEAR(GetDate())*ROUND(RAND()*999999,0)
	            ELSE
		            SET @IsValidKey=0

	            SELECT @IsValidKey as IsValidKey
            End

             */
        }
    }
}
