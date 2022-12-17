import { PricingNodeTypes, Rtos } from "../gql/codegen/graphql";
import { PricingNodePrices } from "./PricingNodePrices";

export type RtoNodeTypePrices = { rto: Rtos; pricingNodeTypeId: PricingNodeTypes; prices: Array<PricingNodePrices> };
