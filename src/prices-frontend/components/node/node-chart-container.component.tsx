import { useQuery } from "@apollo/client";
import { Stack } from "@mui/material";
import _ from "lodash";
import { useCallback, useContext, useEffect, useState, useTransition } from "react";
import { EnumMetadataContext } from "../../contexts/enum-metadata.context";
import { ChangeSpan, PriceMarkets, PricingNode, PricingNodePricesQueryVariables, Query } from "../../gql/codegen/graphql";
import pricingNodePricesQuery from "../../gql/documents/queries/pricingNodePricesQuery";
import { SlotRollingPrice } from "../shared/slot-rolling-price.component";
import { SpanSelector } from "../shared/span-selector.component";
import { Spinner } from "../shared/spinner.component";
import { Change } from "../ticker/change.component";
import NodeChart from "./node-chart.component";

type NodeChartContainerProps = {
    pricingNode: PricingNode;
    subscribeToPriceChanges: Function;
};

export const NodeChartContainer = (props: NodeChartContainerProps) => {
    const { pricingNode, subscribeToPriceChanges } = props;
    const { priceIndexes } = useContext(EnumMetadataContext);
    useEffect(() => subscribeToPriceChanges(), [subscribeToPriceChanges]);

    const [span, setSpan] = useState(ChangeSpan.Day);
    const [busy, startTransition] = useTransition();

    const { loading, data, refetch } = useQuery<Query, PricingNodePricesQueryVariables>(pricingNodePricesQuery, {
        variables: {
            name: pricingNode.name,
            rto: pricingNode.regionalTransmissionOperatorId,
            span: span,
        },
    });

    let pricingNodesWithPrices: Array<PricingNode> = [];
    let percentChange = 0;
    if (data) {
        pricingNodesWithPrices = [data.pricingNodeByRtoAndName as PricingNode];

        const pricesWithPricingNodes = _.flatMap(pricingNodesWithPrices, (pricingNode) =>
            _.map(pricingNode.prices, (price) => {
                const { prices, ...pn } = pricingNode;
                return { ...pn, ...price, __typename: "PricingNodePrices" };
            })
        );

        const realTimePriceIndex = _.filter(priceIndexes, (i) => i.priceMarketId === PriceMarkets.Rtm).map((i) => i.id);

        const oldPrice = _.chain(pricesWithPricingNodes)
            .filter((p) => realTimePriceIndex.indexOf(p.priceIndexId) > -1)
            .sortBy("intervalEndTimeUtc")
            .value()[0].lmpPrice;

        if (oldPrice) {
            percentChange = (pricingNode.currentPrice - oldPrice) / oldPrice;
        }
    }

    const handleSpanChange = useCallback(
        (e: React.MouseEvent<HTMLElement>, value: ChangeSpan) => {
            startTransition(() => {
                setSpan(value);
                refetch({ span: value });
            });
        },
        [refetch]
    );

    return (
        <Stack justifyContent="center" alignItems="center" gap={2}>
            <Stack direction="row" gap={2}>
                <SlotRollingPrice key={pricingNode.id} value={pricingNode.currentPrice} options={{ currency: "USD" }} />
                <Change value={percentChange} />
            </Stack>
            {pricingNodesWithPrices && <SpanSelector span={span} busy={busy} onSpanChange={handleSpanChange} />}
            {loading ? (
                <>
                    <div style={{ alignItems: "center", display: "flex", justifyContent: "center", height: "350px" }}>
                        <Spinner />
                    </div>
                </>
            ) : (
                <NodeChart span={span} pricingNodes={pricingNodesWithPrices ?? []} />
            )}
        </Stack>
    );
};
