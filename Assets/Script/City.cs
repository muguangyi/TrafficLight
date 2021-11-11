using System;
using System.Collections.Generic;
using Taichi.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Light
{
    public sealed class City : ICity
    {
        private Type[] Contexts { get; } = new Type[]
        {
            typeof(Vehicle),
            typeof(TrafficLight),
        };

        private bool start = false;
        private ITrafficLight horizontalLight = null;
        private ITrafficLight verticalLight = null;
        private readonly List<IMovableObjectEmitter> objectEmitters = new List<IMovableObjectEmitter>();
        private bool clicked = false;

        private Transform redLight = null;
        private Transform yellowLight = null;
        private Transform greenLight = null;
        
        private void AddEmitter(IMovableObjectEmitter emitter)
        {
            this.objectEmitters.Add(emitter);
        }

        private void OnResolve()
        {
            Log.Debug("Start the city...");

            SetupUI();
            SetupLights();

            this.horizontalLight = new TrafficLight(5f, 2f, 10f, new Vector2(0f, -1f), false);
            this.verticalLight = new TrafficLight(10f, 2f, 5f, new Vector2(-1f, 4f), true);

            this.horizontalLight.OnLightStateEnter += OnLightStateEnter;
            this.horizontalLight.OnLightStateExit += OnLightStateExit;

            AddEmitter(new VehicleEmitter(this.horizontalLight, 3f, 5f, () => GenerateGameVehicle(true, new Vector2(-25f, 0.5f), 6f)));
            AddEmitter(new VehicleEmitter(this.horizontalLight, 2f, 6f, () => GenerateGameVehicle(true, new Vector2(-28f, -2.5f), 6f)));
            AddEmitter(new VehicleEmitter(this.horizontalLight, 3f, 5f, () => GenerateGameVehicle(true, new Vector2(-30f, -5f), 6f)));
            AddEmitter(new VehicleEmitter(this.verticalLight, 3f, 5f, () => GenerateGameVehicle(false, new Vector2(8f, 20f), 6f)));
            AddEmitter(new VehicleEmitter(this.verticalLight, 2f, 6f, () => GenerateGameVehicle(false, new Vector2(5f, 23f), 6f)));
        }

        private void OnUpdate(float deltaTime)
        {
            HandleUserInput();

            this.horizontalLight?.Update(deltaTime);
            this.verticalLight?.Update(deltaTime);

            for (var i = 0; i < this.objectEmitters.Count; ++i)
            {
                this.objectEmitters[i].Update(deltaTime);
            }
        }

        private IMovableObject GenerateGameVehicle(bool xdir, Vector2 origin, float maxSpeed)
        {
            var light = xdir ? this.horizontalLight : this.verticalLight;
            if (xdir)
            {
                return new XVehicle(origin, maxSpeed, light, !start || UnityEngine.Random.value < 0.7 || SumBadObjectCount() > 2);
            }
            else
            {
                return new YVehicle(origin, maxSpeed, light);
            }
        }

        private int SumBadObjectCount()
        {
            var sum = 0;
            for (var i = 0; i < this.objectEmitters.Count; ++i)
            {
                sum += this.objectEmitters[i].BadObjectCount;
            }

            return sum;
        }

        private void HandleUserInput()
        {
            if (Input.GetMouseButtonDown(0) && !this.clicked)
            {
                this.clicked = true;
                var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var v = new Vector2(pos.x, pos.y);
                for (var i = 0; i < this.objectEmitters.Count; ++i)
                {
                    var emitter = this.objectEmitters[i];
                    var target = emitter.HitTest(v);
                    if (target != null)
                    {
                        target.Click();
                        break;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                this.clicked = false;
            }
        }

        private void SetupUI()
        {
            var root = GameObject.Find("_UI");
            var c = root.transform.Find("Close");
            var closeButton = c.GetComponent<Button>();
            closeButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            var panel = root.transform.Find("StartPanel");
            var startButton = panel.GetComponentInChildren<Button>();
            startButton.onClick.AddListener(() =>
            {
                this.start = true;
                GameObject.Destroy(panel.gameObject);
            });
        }

        private void SetupLights()
        {
            var light = GameObject.Find("_TrafficLight");

            this.redLight = light.transform.Find("Red");
            this.yellowLight = light.transform.Find("Yellow");
            this.greenLight = light.transform.Find("Green");
            
            this.redLight.gameObject.SetActive(false);
            this.yellowLight.gameObject.SetActive(false);
            this.greenLight.gameObject.SetActive(true);
        }

        private void OnLightStateEnter(ITrafficState state)
        {
            switch (state.Type)
            {
                case LightType.Red:
                    this.redLight.gameObject.SetActive(true);
                    break;
                case LightType.Yellow:
                    this.yellowLight.gameObject.SetActive(true);
                    break;
                case LightType.Green:
                    this.greenLight.gameObject.SetActive(true);
                    break;
            }
        }

        private void OnLightStateExit(ITrafficState state)
        {
            switch (state.Type)
            {
                case LightType.Red:
                    this.redLight.gameObject.SetActive(false);
                    break;
                case LightType.Yellow:
                    this.yellowLight.gameObject.SetActive(false);
                    break;
                case LightType.Green:
                    this.greenLight.gameObject.SetActive(false);
                    break;
            }
        }
    }
}