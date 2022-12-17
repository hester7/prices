import { PriceByPricingNode, PricingNode } from "../gql/codegen/graphql";

export type PricingNodePrices = Pick<PricingNode, "id" | "name" | "regionalTransmissionOperatorId" | "pricingNodeTypeId"> & Omit<PriceByPricingNode, "__typename">;

// Ex:
// pricingNodePrices = _.flatMap(data.pricingNodesById as Array<PricingNode>, (pricingNode) =>
// 	_.map(pricingNode.prices, (price) => {
// 		const { prices, ...pn } = pricingNode;
// 		return { ...pn, ...price, __typename: "PricingNodePrices" };
// 	})
// );
