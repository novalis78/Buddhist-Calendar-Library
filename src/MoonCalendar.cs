/*
 * Created by SharpDevelop.
 * User: novalis78
 * Date: 26.01.2005
 * Time: 21:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;

namespace BuddhistCalendar
{
	/// <summary>
	/// standalone moon phase calculation
	/// </summary>
	public class MoonCalendar
	{
		public MoonCalendar()
		{
		}		
		
		private const double PI = 3.1415926535897932384626433832795;
		private const double RAD = 	(PI/180.0);
		private const double SMALL_FLOAT =	(1e-12);
		public struct TimePlace
		{
		    public int year,month,day;
		    public double hour;
		}
		
		#region Moon Algorithm Methods
		public enum MoonPhases {Full, Waning, New, Waxing};

		/*approved :-))*/
		private static void JulianToDate(ref TimePlace now, double jd)
		{
		    long jdi, b;
		    long c,d,e,g,g1;
		
		    jd += 0.5;
		    jdi = (long)jd;
		    if (jdi > 2299160) {
		    	long a = (long)((jdi - 1867216.25)/36524.25);
		        b = jdi + 1 + a - a/4;
		    }
		    else b = jdi;
		
		    c = b + 1524;
		    d = (long)((c - 122.1)/365.25);
		    e = (long)(365.25 * d);
		    g = (long)((c - e)/30.6001);
		    g1 = (long)( 30.6001 * g);
		    now.day = Convert.ToInt32(c - e - g1);
		    now.hour = (jd - jdi) * 24.0;
		    if (g <= 13) now.month = (int)g - 1;
		    else now.month = (int)g - 13;
		    if (now.month > 2) now.year = (int) d - 4716;
		    else now.year = (int) d - 4715;
		}

		/*approved :-)*/
		private static double Julian(int year,int month,double day)
		{
		    /*
		      Returns the number of julian days for the specified day.
		      */
		    
		    int a = 0;
		    int b = 0;
		    int c = 0;
		    int e = 0;
		    
		    if (month < 3)
		    {
				year--;
				month += 12;
		    }
		    if (year > 1582 || (year == 1582 && month>10) ||
			(year == 1582 && month==10 && day > 15)) 
		    {
		    	a = (year/100);
		    	b= 2 - a + (a/4);
		    }
		    c = (int)(365.25* year);
		    e = (int)(30.6001*(month+1));
		    return (b + c + e + day + 1720994.5);
		}

		/*approved :-(*/
		private static double sun_position(double j)
		{
		    double n,x,e,l,dl,v;
		    double i;
		
		    n = Math.Round(360/365.2422*j, 6);
		    i = Convert.ToInt32(n/360);
		    n = n-i*360.0;
		    x = n-3.762863;
		    if (x < 0) x += 360;
		    x *= RAD;
		    e = Math.Round(x,6);
		    
		    do 
		    {
		    	dl = Math.Round(e-0.016718 * Math.Sin(e)-x,6);
		    	e = e-dl/(1-0.016718 * Math.Round(Math.Cos(e),6));
		    } while (Math.Abs(dl)>=SMALL_FLOAT);
		    v = Math.Round(360/PI * Math.Round(Math.Atan(1.01686011182 * Math.Round(Math.Tan(e/2),6)),6),6);
		    l = v+282.596403;
		    i = (int)(l/360);
		    l = l -(i*360.0);
		    return Math.Round(l,6);
		}

		/*approved :-)*/
		private double moon_position(double j, double ls)
		{
		    double ms,l,mm,n,ev,sms,ae,ec;
		    int i;
		    
		    /* ls = sun_position(j) */
		    ms = 0.985647332099*j - 3.762863;
		    if (ms < 0) ms += 360.0;
		    l = 13.176396*j + 64.975464;
		    i = (int)l/360;
		    l = l - i*360.0;
		    if (l < 0) l += 360.0;
		    mm = l-0.1114041*j-349.383063;
		    i = (int)mm/360;
		    mm -= i*360.0;
		    n = 151.950429 - 0.0529539*j;
		    i = (int)n/360;
		    n -= i*360.0;
		    ev = 1.2739*Math.Sin((2*(l-ls)-mm)*RAD);
		    sms = Math.Sin(ms*RAD);
		    ae = 0.1858*sms;
		    mm += ev-ae- 0.37*sms;
		    ec = 6.2886*Math.Sin(mm*RAD);
		    l += ev+ec-ae+ 0.214*Math.Sin(2*mm*RAD);
		    l= 0.6583*Math.Sin(2*(l-ls)*RAD)+l;
		    return Math.Round(l,6);
		}

		/*approved :-)*/
		private double moon_phase(int year,int month,int day, double hour, ref int ip)
		{
		    /*
		      Calculates more accurately than Moon_phase , the phase of the moon at
		      the given epoch.
		      returns the moon phase as a real number (0-1)
		      */
		
		    double j= Julian(year,month,(double)day+hour/24.0)-2444238.5;
		    double ls = sun_position(j);
		    double lm = moon_position(j, ls);
		
		    double t = lm - ls;
		    if (t < 0) t += 360;
		    ip = (int)((t + 22.5)/45) & 0x7;
		    return (1.0 - Math.Cos((lm - ls)*RAD))/2;
		}

		/*approved*/
		private static void nextDay(ref int y, ref int m, ref int d, double dd)
		{
		    TimePlace tp;
		    tp.day = 1;
		    tp.hour = 0;
		    tp.month =1;
		    tp.year = 1900;
		    double jd = Julian(y, m, (double)d);
		    
		    jd += dd;
		    JulianToDate(ref tp, jd);
		    
		    y = tp.year;
		    m = tp.month;
		    d = tp.day;
		}
		#endregion
		
		
		/*approved*/
		public void printMoonTable(int year, int month)
		{
		    int y, m, d;
		    int m0;
		    int h;
		    int begun = 0;
		
		    double pmax = 1;
		    double pmin = 1;
		    int ymax = 0; int mmax = 0; int dmax = 0; int hmax = 0;
		    int ymin = 0; int mmin = 0; int dmin = 0; int hmin = 0;
		    double plast = 0;
		
		    Console.WriteLine("Tabulation of the phase of the moon for one month\n\n");
	
		    y = year;
		    m = month;
		
		    d = 1;
		    m0 = m;
		
		    Console.WriteLine("\nDate\tTime\tPhase\tSegment\n");
		    while(true)
		    {
		        double p = 0;
		        int ip = 0;
		        
		        for (h = 0; h < 24; h++) 
		        {
		            p = moon_phase(y, m, d, h, ref ip);
		
		            if (begun >= 0) {
		                if (p > plast && p > pmax) 
		                {
		                    pmax = p;
		                    ymax = y;
		                    mmax = m;
		                    dmax = d;
		                    hmax = h;
		                }
		                else if (Convert.ToBoolean(pmax))
		                {
		                    Console.WriteLine("{0}/{1}/{2}\t{3}:00          (fullest)",ymax, mmax, dmax, hmax);
		                    pmax = 0;
		                }
		
		                if (p < plast && p < pmin) 
		                {
		                    pmin = p;
		                    ymin = y;
		                    mmin = m;
		                    dmin = d;
		                    hmin = h;
		                }
		                else if (pmin < 1) 
		                {
		                    Console.WriteLine("{0}/{1}/{2}\t{3}:00          (newest)", ymin, mmin, dmin, hmin);
		                    pmin = 1;
		                }
		            
		                if (h == 16) {
		                    Console.WriteLine("{0}/{1}/{2}\t{3}:00\t{4}%\t{5}",
		                           y, m, d, h, Math.Floor(p*1000+0.5)/10, ip);
		                }
		            }
		            else begun = 1;
		
		            plast = p;
		
		        }        
		        nextDay(ref y, ref m, ref d, 1);
		        if (m != m0) break;
		    }
		    return;
		}	
		
		#region Moon Calendar utility methods
		/*retrieves DateTime Object for a given lunar phase in a given month/year*/
		protected DateTime getMoonPhaseDate(int year, int month, MoonPhases phase)
		{
			DateTime a = DateTime.Now;

		    int y, m, d;
		    int m0;
		    int h;
		    int begun = 0;
		    int moon = 2;
		
		    double pmax = 1;
		    double pmin = 1;
		    int ymax = 0; int mmax = 0; int dmax = 0; int hmax = 0;
		    int ymin = 0; int mmin = 0; int dmin = 0; int hmin = 0;
		    double plast = 0;
	
		    y = year;
		    m = month;
		
		    d = 1;
		    m0 = m;
	
		    while(true)
		    {
		        double p = 0;
		        int ip = 0;
		        
		        for (h = 0; h < 24; h++) 
		        {
		            p = moon_phase(y, m, d, h, ref ip);
		
		            if (begun >= 0) {
		                if (p > plast && p > pmax) 
		                {
		                    pmax = p;
		                    ymax = y;
		                    mmax = m;
		                    dmax = d;
		                    hmax = h;
		                    moon = 1;
		                }
		                else if (Convert.ToBoolean(pmax))
		                {
		                    if(MoonPhases.Full == phase && ymax != 0 && mmax != 0 && dmax != 0)
		                    	a = new DateTime(ymax, mmax, dmax, hmax, 0 , 0);
		                    pmax = 0;
		                }
		
		                if (p < plast && p < pmin) 
		                {
		                    pmin = p;
		                    ymin = y;
		                    mmin = m;
		                    dmin = d;
		                    hmin = h;
		                    moon = 2;
		                }
		                else if (pmin < 1) 
		                {
		                    if(MoonPhases.New == phase && ymin != 0 && mmin != 0 && dmin != 0)
		                    	a = new DateTime(ymin, mmin, dmin, hmin, 0, 0);
		                    pmin = 1;
		                }
       					if (h == 16) 
       					{
       						double k = Math.Floor(p*1000+0.5)/10;
       						if(phase == MoonPhases.Waning)
       							if(k > 50.0 && k < 60.0 && moon == 2) a = new DateTime(y, m, d, h, 0, 0) ;
       						if(phase == MoonPhases.Waxing)
       							if(k > 50.0 && k < 60.0 && moon == 1) a = new DateTime(y, m, d, h, 0, 0) ;
		                }		                
		            }
		            else begun = 1;
		
		            plast = p;
		
		        }        
		        nextDay(ref y, ref m, ref d, 1);
		        if (m != m0) break;
		    }		
			return a;
		}
		
		/*returns a list with all full moon dates of a given year*/
		protected ArrayList getFullMoonListForYear(int year)
		{
			ArrayList a = new ArrayList();
			
			for(int i = 1; i <= 12; i++)
			{
				a.Add(getMoonPhaseDate(year, i, MoonPhases.Full));
			}
			return a;
		}
		
		protected ArrayList getWaningMoonListForYear(int year)
		{
			ArrayList a = new ArrayList();
			
			for(int i = 1; i <= 12; i++)
			{
				a.Add(getMoonPhaseDate(year, i, MoonPhases.Waning));
			}
			return a;
		}
		
		protected ArrayList getWaxingMoonListForYear(int year)
		{
			ArrayList a = new ArrayList();
			
			for(int i = 1; i <= 12; i++)
			{
				a.Add(getMoonPhaseDate(year, i, MoonPhases.Waxing));
			}
			return a;
		}		
		
		protected ArrayList getNewMoonListForYear(int year)
		{
			ArrayList a = new ArrayList();
			
			for(int i = 1; i < 12; i++)
			{
				a.Add(getMoonPhaseDate(year, i, MoonPhases.New));
			}
			return a;
		}
		#endregion
	}
}
