import { PricingNode, PricingNodeTypes, Rtos } from "../gql/codegen/graphql";

export type RtoNodeTypePricingNodes = { rto: Rtos; pricingNodeTypeId: PricingNodeTypes; pricingNodes: Array<PricingNode> };
