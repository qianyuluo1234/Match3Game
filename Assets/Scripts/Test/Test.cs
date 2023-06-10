using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Match3Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class Test : MonoBehaviour
    {
        [SerializeField]
        private Text _text;
        private void Awake()
        {
            
        }

        private void Start()
        {
            Match3Data match3Data = new Match3Data(10, 9, new[] {1, 2, 3, 4});
            Match3Utility.CreateMap(match3Data);
            ShowMap(match3Data);
            GetAllEliminationBlocksAsync(match3Data);
        }

        private void ShowMap(Match3Data match3Data)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < match3Data.row; i++)
            {
                for (int j = 0; j < match3Data.column; j++)
                {
                    builder.Append(match3Data.map[i,j]).Append(" , ");
                }

                builder.Remove(builder.Length - 3, 3);
                builder.Append('\n');
            }

            _text.text = builder.ToString();
        }
        private async Task GetAllEliminationBlocksAsync(Match3Data match3Data)
        {
            Debug.Log("thread id:" + Thread.CurrentThread.ManagedThreadId);

            List<EliminationBlocks> blockList = await Task.Run(() => Match3Utility.FindAllEliminationBlocks(match3Data.map));
            foreach (var block in blockList)
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < block.count; i++)
                {
                    builder.Append(block[i]).Append(',');
                }
                Debug.Log(builder.ToString());
            }
        }
    }
}