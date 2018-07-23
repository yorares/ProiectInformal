using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stark.Models;

namespace Stark.Controllers
{
    public class ReportsController : Controller
    {
        private readonly starkContext _context;

        public ReportsController(starkContext context)
        {
            _context = context;
        }

        //Counts the number of reviews for a specific License Plate - passed as a parameter
        public int Counter(string plate)
        {
            int counter;
            counter = _context.Review.Count(s => s.Licence.Plate == plate);
            return counter;
        }
        //Counts the number of reviews for a specific License Id, badge type and optional wight - passed as parameters
        public double Counter(int id, int type, int weight=1)
        {
            double counter;
            counter = _context.Review.Count(s => s.Badge.Type == type && s.LicenceId == id);
            return counter*type*weight;
        }

        //Generates a key value pair list - License plate as key and the score as value
        //Score - is a weighted average - cont
        //Each review has a type - an int value  - 0 for the worst, 1 for the bad , 3 for the good and 4 for the excellent - cont
        //For each plate, all its reviews are added and the sum is then divided by the number of reviews for said plate - default weight is 1.
        
        public IDictionary<string, double> GetScore()
        {
            IDictionary<string, double> scores = new Dictionary<string, double>();

            foreach (var item in _context.Cars)
            {
                scores.Add(item.Plate, Math.Round((Counter(item.LicenceId, 0) + Counter(item.LicenceId, 1) + Counter(item.LicenceId, 3) + Counter(item.LicenceId, 4))/Counter(item.Plate), 2));

            }
            return scores;
        }

        //Retrives the Id of a specific license plate  - passed as a parameter
        public int GetId(string plate)
        {
            int id;
            Cars ob = _context.Cars.Where(s=>s.Plate==plate).FirstOrDefault();
            id = ob.LicenceId;
            return id;
        }

        //Main method that instances the view
        public IActionResult Index()
        {
            var ScoreList = GetScore().ToList();
            ScoreList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); //sorting the view by descending score

            //List of object of the ViewModel "CarsViewcs"
            List<CarsViewcs> All=new List<CarsViewcs>();

            int i = 1; //used to memorise the position in the ranking

            //Adding all the records 
            foreach (var item in ScoreList)
            {
                CarsViewcs c = new CarsViewcs { LicenceId=GetId(item.Key), Position=i, Plate = item.Key, ReviewCount = Counter(item.Key), Score = item.Value };
                if (c.ReviewCount > 0)
                    {
                    All.Add(c);
                    ++i;
                }

            }
            return View(All);
        }
    }
}