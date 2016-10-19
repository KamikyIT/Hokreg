using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Uniso.InStat.Models;

namespace Uniso.InStat.Gui.Forms
{
    public partial class MyViolationForm : Form
    {
        public MyViolationForm()
        {
            InitializeComponent();


            rb2P.Checked = false;
            rb1P.Checked = false;
            rb0P.Checked = false;

            checkBox1.Enabled = true;
            
            foulPlayersCount = FoulTypeEnum.None;

            UpdateUI();

            ResetFoulTypeChecks();
        }

        private void UpdateUI()
        {
            SetRadioButtonEnabled(rb1);
            SetRadioButtonEnabled(rb2);
            SetRadioButtonEnabled(rb3);
            SetRadioButtonEnabled(rb4);
            SetRadioButtonEnabled(rb5);
            SetRadioButtonEnabled(rb6);
            SetRadioButtonEnabled(rb7);
            SetRadioButtonEnabled(rb8);
            SetRadioButtonEnabled(rb9);
            SetRadioButtonEnabled(rb10);
            SetRadioButtonEnabled(rb11);
            SetRadioButtonEnabled(rb12);
            SetRadioButtonEnabled(rb13);
            SetRadioButtonEnabled(rb14);

            button1.Enabled = action_type > 0;
        }

        private void SetRadioButtonEnabled(RadioButton rb)
        {
            // Если еще не выбрал количетво игроков в фоле
            if (foulPlayersCount == FoulTypeEnum.None)
            {
                rb.Enabled = false;
                return;
            }

            var tag = rb.Tag.ToString();

            var rb_action_type = int.Parse(tag);

            var stages = MarkersWomboCombo.GetFoulMarkerPossibleStages(rb_action_type);

            if (foulPlayersCount == FoulTypeEnum.NoPlayer)
            {
                rb.Enabled = stages.Contains(MarkersWomboCombo.FoulStageEnum.Player0);
            }
            if (foulPlayersCount == FoulTypeEnum.SoloPLayer)
            {
                rb.Enabled = stages.Contains(MarkersWomboCombo.FoulStageEnum.Player1);
            }
            if (foulPlayersCount == FoulTypeEnum.TwoPlayers)
            {
                rb.Enabled = stages.Contains(MarkersWomboCombo.FoulStageEnum.Player2);
            }
        }

        private void ResetFoulTypeChecks()
        {
            this.action_type = 0;

            rb1.Checked = false;
            rb2.Checked = false;
            rb3.Checked = false;
            rb4.Checked = false;
            rb5.Checked = false;
            rb6.Checked = false;
            rb7.Checked = false;
            rb8.Checked = false;
            rb9.Checked = false;
            rb10.Checked = false;
            rb11.Checked = false;
            rb12.Checked = false;
            rb13.Checked = false;
            rb14.Checked = false;
        }

        private void radioButtonFoulPlayersCount_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (radioButton.Checked)
                {
                    var tag = radioButton.Tag.ToString();

                    this.foulPlayersCount = (FoulTypeEnum)int.Parse(tag);

                    UpdateUI();

                    ResetFoulTypeChecks();
                }
            }
        }

        private void radioButtonFoulType_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (radioButton.Checked)
                {
                    var tag = radioButton.Tag.ToString();

                    this.action_type = int.Parse(tag);

                    UpdateUI();
                }
            }
        }

        private int action_type { get; set; }

        private bool IsPair { get { return this.checkBox1.Checked; } }

        private FoulTypeEnum foulPlayersCount { get; set; }

        public void Result(out int action_type, out List<MarkersWomboCombo.FoulStageEnum> stages, out bool pair)
        {
            action_type = this.action_type;

            stages = GetStages();

            pair = this.IsPair;
        }

        private List<MarkersWomboCombo.FoulStageEnum> GetStages()
        {
            var res = MarkersWomboCombo.GetFoulMarkerPossibleStages(this.action_type);

            switch (foulPlayersCount)
            {
                case FoulTypeEnum.TwoPlayers:
                    break;
                case FoulTypeEnum.SoloPLayer:
                    if (res.Contains(MarkersWomboCombo.FoulStageEnum.Player2))
                    {
                        res.Remove(MarkersWomboCombo.FoulStageEnum.Player2);
                    }
                    break;
                case FoulTypeEnum.NoPlayer:
                    if (res.Contains(MarkersWomboCombo.FoulStageEnum.Player2))
                    {
                        res.Remove(MarkersWomboCombo.FoulStageEnum.Player2);
                    }
                    if (res.Contains(MarkersWomboCombo.FoulStageEnum.Player1))
                    {
                        res.Remove(MarkersWomboCombo.FoulStageEnum.Player1);
                    }

                    if (res.Contains(MarkersWomboCombo.FoulStageEnum.Player0) == false)
                    {
                        res.Add(MarkersWomboCombo.FoulStageEnum.Player0);
                    }
                    break;
                case FoulTypeEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return res;
        }

        public enum FoulTypeEnum
        {
            TwoPlayers = 2,
            SoloPLayer = 1,
            NoPlayer = 0,
            None = -1,
        }
    }


}
