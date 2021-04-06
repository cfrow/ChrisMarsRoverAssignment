using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChrisMarsRoverAssignmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarsRoverController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> MoveRover(RoverInstruction instruction)
        {
            var xBound = int.Parse(instruction.GridBounds.Substring(0, 1));
            var yBound = int.Parse(instruction.GridBounds.Substring(1, 1));

            var xMove = int.Parse(instruction.PositionHeading.Substring(0, 1));
            var yMove = int.Parse(instruction.PositionHeading.Substring(1, 1));

            if (xMove > xBound || yMove > yBound)
            {
                return BadRequest("Current position provided is outside the bounds of the defined grid.");
            }

            char[] movements = instruction.Movement.ToCharArray();

            List<string> cardinalPoints = new List<string> { "N", "E", "S", "W" };

            string currentHeading = instruction.PositionHeading.Substring(2, 1).ToUpper();
            foreach (var move in movements)
            {
                int cardinalIndex = cardinalPoints.IndexOf(currentHeading);

                switch (move)
                {
                    case 'L':
                    case 'l':
                        currentHeading = cardinalIndex == 0 ? cardinalPoints[3] : cardinalPoints[cardinalIndex - 1];

                        break;
                    case 'R':
                    case 'r':
                        currentHeading = cardinalIndex == 3 ? currentHeading = cardinalPoints[0] : currentHeading = cardinalPoints[cardinalIndex + 1];

                        break;
                    case 'M':
                    case 'm':
                        var adjustment = AdjustMove(currentHeading);

                        if (currentHeading == "N" || currentHeading == "S")
                        {
                            yMove += adjustment;
                        }
                        else
                        {
                            xMove += adjustment;
                        }

                        if (yMove < 0 || xMove < 0 || yMove > yBound || xMove > xBound)
                        {
                            return BadRequest("Requested movement will take rover out of bounds of the defined grid.");
                        }

                        break;
                }
            }

            return Ok($"Rover successfully moved. Current position and heading are {xMove}{yMove}{currentHeading}");
        }

        private int AdjustMove(string heading)
        {
            var adjust = 0;
            switch (heading)
            {
                case "N":
                    adjust++;
                    break;
                case "E":
                    adjust++;
                    break;
                case "S":
                    adjust--;
                    break;
                case "W":
                    adjust--;
                    break;
            }

            return adjust;
        }
    }
}
