import { Ticker } from "../ticker/ticker.component";
import { TickerItem } from "../ticker/ticker-item.component";
import { PricingNode } from "../../gql/codegen/graphql";
import { memo, useEffect } from "react";

type NodeTickerProps = {
    pricingNodes: Array<PricingNode>;
    subscribeToPriceChanges: Function;
};

export const NodeTicker = memo<NodeTickerProps>(function NodeTicker(props: NodeTickerProps) {
    const { pricingNodes, subscribeToPriceChanges } = props;
    useEffect(() => subscribeToPriceChanges(), [subscribeToPriceChanges]);

    return (
        <Ticker>
            {pricingNodes?.map((pricingNode) => (
                <TickerItem key={pricingNode.id} pricingNode={pricingNode} />
            ))}
        </Ticker>
    );
});
