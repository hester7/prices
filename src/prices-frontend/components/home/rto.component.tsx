import { useQuery } from "@apollo/client";
import { Grid, Stack, Typography } from "@mui/material";
import router from "next/router";
import { useCallback, useState, useTransition } from "react";
import { ChangeSpan, PricingNode, PricingNodesPricesQueryVariables, PricingNodeTypes, Query, Rtos } from "../../gql/codegen/graphql";
import pricingNodesPricesQuery from "../../gql/documents/queries/pricingNodesPricesQuery";
import NodeChart from "../node/node-chart.component";
import { SpanSelector } from "../shared/span-selector.component";
import { Spinner } from "../shared/spinner.component";
import { NodesList } from "./nodes-list.component";

type RtoProps = {
    rto: Rtos;
    pricingNodeTypeId: PricingNodeTypes;
    pricingNodes: Array<PricingNode>;
    subscribeToPriceChanges: Function;
};

export const Rto = (props: RtoProps) => {
    const { rto, pricingNodeTypeId, pricingNodes, subscribeToPriceChanges } = props;
    const [span, setSpan] = useState(ChangeSpan.Day);
    const [selectedNodes, setSelectedNodes] = useState<Array<PricingNode>>([]);
    const [busy, startTransition] = useTransition();

    const nodeClickHandler = (rtoId: Rtos, node: string) => {
        router.push(`/nodes/${rtoId}/${node}`);
    };

    const { loading, data, refetch } = useQuery<Query, PricingNodesPricesQueryVariables>(pricingNodesPricesQuery, {
        variables: {
            ids: pricingNodes.map((pn) => pn.id),
            span: span,
        },
    });

    let pricingNodesWithPrices: Array<PricingNode> = [];
    if (data) {
        pricingNodesWithPrices = data.pricingNodesById as Array<PricingNode>;
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

    const handleNodeChecked = useCallback(
        (pricingNodes: Array<PricingNode>) => {
            setSelectedNodes(pricingNodes);
            refetch({ span });
        },
        [refetch, span]
    );

    return (
        <>
            <Grid container spacing={2} justifyContent="center" alignItems="center" sx={{ marginTop: "8px" }}>
                <Grid
                    item
                    xs={12}
                    sx={(theme) => ({
                        backgroundColor: theme.palette.background["neutral"],
                        padding: "8px",
                    })}
                >
                    <Typography
                        variant="h3"
                        fontWeight={600}
                        sx={(theme) => ({
                            fontFamily: theme.typography["fontFamilyAlt"],
                        })}
                    >
                        {rto} - {pricingNodeTypeId}
                    </Typography>
                </Grid>
                <Grid item xs={12} sm={3} xl={2} sx={{ minWidth: "300px" }}>
                    <NodesList
                        pricingNodes={pricingNodes}
                        selectedNodes={selectedNodes}
                        onNodeClick={nodeClickHandler}
                        onSelectionChange={handleNodeChecked}
                        subscribeToPriceChanges={subscribeToPriceChanges}
                    />
                </Grid>
                <Grid item xs={12} sm={9} xl={10}>
                    <Stack justifyContent="center" alignItems="center" gap={2} width="100%">
                        {pricingNodesWithPrices && <SpanSelector span={span} busy={busy} onSpanChange={handleSpanChange} />}
                        <div style={{ alignItems: "center", display: "flex", justifyContent: "center", height: "350px" }}>
                            {loading ? (
                                <Spinner />
                            ) : (
                                <NodeChart span={span} pricingNodes={pricingNodesWithPrices ?? []} includeAverages={true} selectedNodes={selectedNodes} />
                            )}
                        </div>
                    </Stack>
                </Grid>
            </Grid>
        </>
    );
};
