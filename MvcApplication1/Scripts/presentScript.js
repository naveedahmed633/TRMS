{
    "type": "pie", 
 	"title":{
 	    "text": "Earnings Insights<br>Operating System",
        "align": "right",
        "fontColor": "#616161"
 	},
 	"legend":{
 	    "text":"%t<br>",
        "width": 120,
        "verticalAlign": "middle",
        "borderWidth": 0,
        "toggleAction": "remove",
        "item":{
            "padding": 5,
          "borderRadius": 3,
          "fontColor": "#fff",
          "align": "right",
          "width": 100
        },
 	  "header":{
 	      "text":"OS Products",
          "align": "right",
          "fontSize": 13,
          "bold": true,
          "fontColor": "#616161"
 	  },
 	  "itemOff":{
 	      "alpha": 0.7,
          "textAlpha": 1,
          "fontColor": "#616161",
          "textDecoration": "line-through",
          "lineWidth": 2
 	  },
 	  "markerOff":{
 	      "alpha": 0.2
 	  }
 	},
 	"plot":{
 	    "refAngle": 270,
        "decimals": 0,
        "thousandsSeparator": ",",
        "format": "$%v",
        "valueBox":{
            "decimals": 2
        },
 	  "animation":{
 	      "effect": 3,
          "method": 1,
          "sequence": 1,
          "onLegendToggle": false
 	  }
 	},
 	"tooltip":{
 	    "text":"%t<br>$%v",
        "placement": "node:out",
        "offsetR": 10,
        "width": 110,
        "fontColor": "#fff",
        "borderRadius": 3,
        "bold": true,
        "align": "right"
 	},
  "scale":{
      "sizeFactor": 0.75
  },
	"series" : [
		{
		    "values" : [@Model.lineManager.dasboardElement.onTime],
		    "text":"OEM Licensing",
		    "backgroundColor": "#512DA8",
		    "legendText": "%t<br><b>$%v</b>",
		    "legendMarker":{
		        "type": "circle",
		        "size": 7,
		        "borderColor": "#512DA8",
		        "borderWidth": 4,
		        "backgroundColor":"#fff"
		    },
		    "legendItem":{
		        "backgroundColor": "#512DA8"
		    },
		    "tooltip":{
		        "backgroundColor": "#512DA8"
		    }
		},
		{
		    "values" : [21451879],
		    "text":"Mobile Platforms",
		    "backgroundColor": "#5E35B1",
		    "legendText": "%t<br><b>$%v</b>",
		    "legendMarker":{
		        "type": "circle",
		        "size": 7,
		        "borderColor": "#5E35B1",
		        "borderWidth": 4,
		        "backgroundColor":"#fff"
		    },
		    "legendItem":{
		        "backgroundColor": "#5E35B1"
		    },
		    "tooltip":{
		        "backgroundColor": "#5E35B1"
		    }
		},
		{
		    "values" : [20457965],
		    "text":"Support Services",
		    "backgroundColor": "#673AB7",
		    "legendText": "%t<br><b>$%v</b>",
		    "legendMarker":{
		        "type": "circle",
		        "size": 7,
		        "borderColor": "#673AB7",
		        "borderWidth": 4,
		        "backgroundColor":"#fff"
		    },
		    "legendItem":{
		        "backgroundColor": "#673AB7"
		    },
		    "tooltip":{
		        "backgroundColor": "#673AB7"
		    }
		},
		{
		    "values" : [19989878],
		    "text":"License Upgrades",
		    "backgroundColor": "#7E57C2",
		    "legendText": "%t<br><b>$%v</b>",
		    "legendMarker":{
		        "type": "circle",
		        "size": 7,
		        "borderColor": "#7E57C2",
		        "borderWidth": 4,
		        "backgroundColor":"#fff"
		    },
		    "legendItem":{
		        "backgroundColor": "#7E57C2"
		    },
		    "tooltip":{
		        "backgroundColor": "#7E57C2"
		    }
		},
		{
		    "values" : [18521179],
		    "text":"Other",
		    "backgroundColor": "#9575CD",
		    "legendText": "%t<br><b>$%v</b>",
		    "legendMarker":{
		        "type": "circle",
		        "size": 7,
		        "borderColor": "#9575CD",
		        "borderWidth": 4,
		        "backgroundColor":"#fff"
		    },
		    "legendItem":{
		        "backgroundColor": "#9575CD"
		    },
		    "tooltip":{
		        "backgroundColor": "#9575CD"
		    }
		}
	]
}