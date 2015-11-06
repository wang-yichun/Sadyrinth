// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by the Game Data Editor.
//
//      Changes to this file will be lost if the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;

using GameDataEditor;

namespace GameDataEditor
{
    public class GDECommonData : IGDEData
    {
        private static string show_remain_fuelKey = "show_remain_fuel";
		private bool _show_remain_fuel;
        public bool show_remain_fuel
        {
            get { return _show_remain_fuel; }
            set {
                if (_show_remain_fuel != value)
                {
                    _show_remain_fuel = value;
                    GDEDataManager.SetBool(_key+"_"+show_remain_fuelKey, _show_remain_fuel);
                }
            }
        }

        private static string long_tap_stage_record_clearKey = "long_tap_stage_record_clear";
		private bool _long_tap_stage_record_clear;
        public bool long_tap_stage_record_clear
        {
            get { return _long_tap_stage_record_clear; }
            set {
                if (_long_tap_stage_record_clear != value)
                {
                    _long_tap_stage_record_clear = value;
                    GDEDataManager.SetBool(_key+"_"+long_tap_stage_record_clearKey, _long_tap_stage_record_clear);
                }
            }
        }

        private static string soundKey = "sound";
		private bool _sound;
        public bool sound
        {
            get { return _sound; }
            set {
                if (_sound != value)
                {
                    _sound = value;
                    GDEDataManager.SetBool(_key+"_"+soundKey, _sound);
                }
            }
        }

        private static string musicKey = "music";
		private bool _music;
        public bool music
        {
            get { return _music; }
            set {
                if (_music != value)
                {
                    _music = value;
                    GDEDataManager.SetBool(_key+"_"+musicKey, _music);
                }
            }
        }

        private static string world_countKey = "world_count";
		private int _world_count;
        public int world_count
        {
            get { return _world_count; }
            set {
                if (_world_count != value)
                {
                    _world_count = value;
                    GDEDataManager.SetInt(_key+"_"+world_countKey, _world_count);
                }
            }
        }

        private static string versionKey = "version";
		private string _version;
        public string version
        {
            get { return _version; }
            set {
                if (_version != value)
                {
                    _version = value;
                    GDEDataManager.SetString(_key+"_"+versionKey, _version);
                }
            }
        }

        private static string auto_selected_stage_idKey = "auto_selected_stage_id";
		private string _auto_selected_stage_id;
        public string auto_selected_stage_id
        {
            get { return _auto_selected_stage_id; }
            set {
                if (_auto_selected_stage_id != value)
                {
                    _auto_selected_stage_id = value;
                    GDEDataManager.SetString(_key+"_"+auto_selected_stage_idKey, _auto_selected_stage_id);
                }
            }
        }

        private static string stageKey = "stage";
		public List<GDEStageData>      stage;
		public void Set_stage()
        {
	        GDEDataManager.SetCustomList(_key+"_"+stageKey, stage);
		}
		

        public GDECommonData()
		{
			_key = string.Empty;
		}

		public GDECommonData(string key)
		{
			_key = key;
		}
		
        public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
        {
            _key = dataKey;

			if (dict == null)
				LoadFromSavedData(dataKey);
			else
			{
                dict.TryGetBool(show_remain_fuelKey, out _show_remain_fuel);
                dict.TryGetBool(long_tap_stage_record_clearKey, out _long_tap_stage_record_clear);
                dict.TryGetBool(soundKey, out _sound);
                dict.TryGetBool(musicKey, out _music);
                dict.TryGetInt(world_countKey, out _world_count);
                dict.TryGetString(versionKey, out _version);
                dict.TryGetString(auto_selected_stage_idKey, out _auto_selected_stage_id);

                dict.TryGetCustomList(stageKey, out stage);
                LoadFromSavedData(dataKey);
			}
		}

        public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			
            _show_remain_fuel = GDEDataManager.GetBool(_key+"_"+show_remain_fuelKey, _show_remain_fuel);
            _long_tap_stage_record_clear = GDEDataManager.GetBool(_key+"_"+long_tap_stage_record_clearKey, _long_tap_stage_record_clear);
            _sound = GDEDataManager.GetBool(_key+"_"+soundKey, _sound);
            _music = GDEDataManager.GetBool(_key+"_"+musicKey, _music);
            _world_count = GDEDataManager.GetInt(_key+"_"+world_countKey, _world_count);
            _version = GDEDataManager.GetString(_key+"_"+versionKey, _version);
            _auto_selected_stage_id = GDEDataManager.GetString(_key+"_"+auto_selected_stage_idKey, _auto_selected_stage_id);

            stage = GDEDataManager.GetCustomList(_key+"_"+stageKey, stage);
         }

        public void Reset_show_remain_fuel()
        {
            GDEDataManager.ResetToDefault(_key, show_remain_fuelKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetBool(show_remain_fuelKey, out _show_remain_fuel);
        }

        public void Reset_long_tap_stage_record_clear()
        {
            GDEDataManager.ResetToDefault(_key, long_tap_stage_record_clearKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetBool(long_tap_stage_record_clearKey, out _long_tap_stage_record_clear);
        }

        public void Reset_sound()
        {
            GDEDataManager.ResetToDefault(_key, soundKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetBool(soundKey, out _sound);
        }

        public void Reset_music()
        {
            GDEDataManager.ResetToDefault(_key, musicKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetBool(musicKey, out _music);
        }

        public void Reset_world_count()
        {
            GDEDataManager.ResetToDefault(_key, world_countKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetInt(world_countKey, out _world_count);
        }

        public void Reset_version()
        {
            GDEDataManager.ResetToDefault(_key, versionKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(versionKey, out _version);
        }

        public void Reset_auto_selected_stage_id()
        {
            GDEDataManager.ResetToDefault(_key, auto_selected_stage_idKey);

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            dict.TryGetString(auto_selected_stage_idKey, out _auto_selected_stage_id);
        }

        public void Reset_stage()
		{
			GDEDataManager.ResetToDefault(_key, stageKey);

			Dictionary<string, object> dict;
			GDEDataManager.Get(_key, out dict);

			dict.TryGetCustomList(stageKey, out stage);
			stage = GDEDataManager.GetCustomList(_key+"_"+stageKey, stage);

			stage.ForEach(x => x.ResetAll());
		}

        public void ResetAll()
        {
            GDEDataManager.ResetToDefault(_key, versionKey);
            GDEDataManager.ResetToDefault(_key, world_countKey);
            GDEDataManager.ResetToDefault(_key, stageKey);
            GDEDataManager.ResetToDefault(_key, auto_selected_stage_idKey);
            GDEDataManager.ResetToDefault(_key, show_remain_fuelKey);
            GDEDataManager.ResetToDefault(_key, long_tap_stage_record_clearKey);
            GDEDataManager.ResetToDefault(_key, soundKey);
            GDEDataManager.ResetToDefault(_key, musicKey);

            Reset_stage();

            Dictionary<string, object> dict;
            GDEDataManager.Get(_key, out dict);
            LoadFromDict(_key, dict);
        }
    }
}
