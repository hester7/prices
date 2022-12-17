import _ from "lodash";
import { PricingNode, PricingNodeTypes, Rtos } from "../../gql/codegen/graphql";

const colors = [
    "#4263f5",
    "#09A69F",
    "#666F7E",
    "#3A93BB",
    "#CACED9",
    "#94E4B1",
    "#E59F01",
    "#CC79A7",
    "#AADCF3",
    "#7B399B",
    "#8073F7",
    "#019E73",
    "#000000",
    "#01A0E1",
    "#00716B",
    "#B05CD8",
    "#0070D2",
    "#E3CE7D",
    "#93C9F8",
    "#C33936",
    "#17325D",
    "#61B17E",
    "#75DED9",
    "#FFB65D",
    "#98A2AE",
    "#E3B1FA",
];

export const getColors = () => colors;

export const getRandomColors = () => _.shuffle(colors);

export const getRandomColor = () => _.shuffle(colors)[0];

export const getNodeColor = (pricingNode: PricingNode) => {
    switch (pricingNode?.regionalTransmissionOperatorId) {
        case Rtos.Caiso:
            {
                switch (pricingNode.pricingNodeTypeId) {
                    case PricingNodeTypes.Hub:
                        return "#1C495F";

                    case PricingNodeTypes.Dlap:
                        return "#559C9E";
                }
            }
            break;
        case Rtos.Ercot:
            {
                switch (pricingNode.pricingNodeTypeId) {
                    case PricingNodeTypes.Hub:
                        return "#3A7C89";

                    case PricingNodeTypes.Dlap:
                        return "#7CBCB0";
                }
            }
            break;
    }

    return getRandomColor();
};

export const getRtoColor = (pricingNode: PricingNode) => {
    switch (pricingNode?.regionalTransmissionOperatorId) {
        case Rtos.Caiso:
            return "#AADCF3";
        case Rtos.Ercot:
            return "#8073F7";
        default:
            return getRandomColor();
    }
};
